using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Primitive;
using System.Linq;
using ApiServer.Models;

namespace ApiServer
{
	public class WorkerManager
	{
		private static WorkerManager _instance = new WorkerManager();

		public static WorkerManager Instance => _instance;

		/// <summary>
		/// Api settings for search modules
		/// </summary>
		private SearchEngineApiSettings _apiSettings;

		/// <summary>
		/// redis distributed cache for search results
		/// </summary>
		private ICacheModule _cache;

		/// <summary>
		/// List of available search modules
		/// </summary>
		private List<ISearchModule> _searchModules = new List<ISearchModule>();

		private static char[] Separators = new char[] { ' ' };

		public void Initialize(SearchEngineApiSettings apiSettings, ICacheModule cache)
		{
			// inject IOptions
			_apiSettings = apiSettings;

			// inject cache
			_cache = cache;

			TryAddSearchModule(GoogleCustomSearchModule.Instance);
		}

		/// <summary>
		/// Try to activate search module and add to available search module list.
		/// </summary>
		/// <param name="searchModule">target search module</param>
		/// <returns>true if successfully added</returns>
		private bool TryAddSearchModule(ISearchModule searchModule)
		{
			// error: already added
			if (_searchModules.Contains(searchModule))
			{
				return false;
			}

			// error: couldn't initialize
			if (!searchModule.Initialize(_apiSettings))
			{
				return false;
			}

			_searchModules.Add(searchModule);

			return true;
		}

		private ISearchModule DecideSearchModule()
		{
			if (_searchModules == null || _searchModules.Count == 0)
			{
				return null;
			}

			for (int i = _searchModules.Count - 1; i >= 0; --i)
			{
				if (!_searchModules[i].IsAvailable)
				{
					continue;
				}

				return _searchModules[i];
			}

			return null;
		}

		// 블럭된 키워드중에 TTL이 지난 키워드는 해제한다.
		// MaxBlockedWords가 현재는 4개 뿐이기에 순회함. 더 커지면 이/진탐색 등으로 개선할 것.
		private string RefreshBlockedKeywords(ref Dictionary<string, DateTime> blocked)
		{
			string oldest = null;

			// 블럭된 키워드중에 TTL이 지난 키워드는 해제한다.
			// MaxBlockedWords가 현재는 4개 뿐이기에 순회함. 더 커지면 이/진탐색 등으로 개선할 것.
			foreach (var keyword in blocked.Keys)
			{
				var expireAt = blocked[keyword];

				// 만료되었다면 삭제
				if (expireAt.CompareTo(DateTime.Now) <= 0)
				{
					blocked.Remove(keyword);

					continue;
				}

				// 가장 오래된 블록 키워드 갱신
				if (oldest == null || blocked[oldest].CompareTo(expireAt) > 0)
				{
					oldest = keyword;
				}
			}

			return oldest;
		}

		/// <summary>
		/// searchKeyword에서 blockKeyword가 연관검색어로 검색되지 않도록 블럭한다.
		/// </summary>
		/// <param name="searchKeyword">부모 키워드</param>
		/// <param name="blockKeyword">블럭할 키워드</param>
		public async Task BlockAssociativeKeyword(string searchKeyword, string blockKeyword)
		{
			SearchResultCache cached = await _cache.GetAsync(searchKeyword);

			// 캐시된 적 없거나, 연관 키워드에 블럭할 키워드가 없다면 스킵
			if (cached == null || !cached.AssociativeWords.Contains(blockKeyword))
			{
				return;
			}

			var blocked = cached.BlockedWords;

			var oldest = RefreshBlockedKeywords(ref blocked);

			// 만료 기한 갱신
			blocked[blockKeyword] = DateTime.Now + Constants.blockExpireAfter;

			// 최대 블럭 개수에 도달하면 가장 오래된 키워드를 해제한다.
			if (blocked.Count > Constants.MaxBlockedWords)
			{
				blocked.Remove(oldest);
			}

			cached.BlockedWords = blocked;

			await _cache.SetAsync(searchKeyword, cached);
		}

		public async Task<SearchResultCache> GetResult(string keyword)
		{
			SearchResultCache cached = await _cache.GetAsync(keyword);

			// 검색 결과 캐시가 없다면
			if (cached == null)
			{
				cached = new SearchResultCache();

				// TODO: handling for not found available module
				cached.Results = DecideSearchModule()?.Search(keyword);

				var scores = new ConcurrentDictionary<string, uint>();

				// TODO: use worker
				foreach (var result in cached.Results)
				{
					if (result.Snippet == null)
					{
						continue;
					}

					var tokens = new StringTokenizer(result.Snippet, Separators);

					tokens.Score(ref scores);
				}

				var sorted = scores.OrderByDescending(item => item.Value);

				cached.AssociativeWords = sorted.Take(Constants.MaxAssociativeWords).Select(x => x.Key).ToList();
				cached.BlockedWords = new Dictionary<string, DateTime>();

				// TODO: search failed handling
				await _cache.SetAsync(keyword, cached);
			}
			else if (cached.BlockedWords.Count > 0)
			{
				var blocked = cached.BlockedWords;

				// 블럭된 검색어 갱신
				RefreshBlockedKeywords(ref blocked);

				cached.BlockedWords = blocked;
			}

			return cached;
		}

		public async Task RemoveCachedResult(string keyword)
		{
			await _cache.RemoveAsync(keyword);
		}
	}
}
