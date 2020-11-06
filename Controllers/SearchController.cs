using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using ApiServer.Models;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Primitive;
using System.Collections.Concurrent;
using System.Linq;

namespace ApiServer.Controllers
{
	[ApiController]
	[Route("search")]
	public class SearchController : ControllerBase
	{
		/// <summary>
		/// api settings for search modules
		/// </summary>
		private readonly SearchEngineApiSettings _apiSettings;

		public SearchController(IOptions<SearchEngineApiSettings> apiSettingsAccessor)
		{
			// inject IOptions
			_apiSettings = apiSettingsAccessor.Value;

			SearchManager.Instance.Initialize(_apiSettings);
		}

		private static char[] Separators = new char[]{' '};

		/// <summary>
		/// Get result of search by {keyword}
		///
		/// injected cache dependency via FromServices attribute.
		/// </summary>
		/// <param name="keyword">search target</param>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<SearchResultDTO>> GetSearchResult([FromServices] ICacheManager cache, string keyword)
		{
			SearchResultCache cached = await cache.GetAsync(keyword);

			// 검색 결과 캐시가 없다면
			if (cached == null)
			{
				cached = new SearchResultCache();

				cached.Results = SearchManager.Instance.Search(keyword);

				var scores = new ConcurrentDictionary<string, uint>();

				// TODO: use worker
				foreach (var result in cached.Results)
				{
					var tokens = new StringTokenizer(result.Snippet, Separators);

					tokens.Score(ref scores);
				}

				var sorted = scores.OrderByDescending(item => item.Value);

				cached.AssociativeWords = sorted.Take(5).Select(x => x.Key).ToList();;

				// TODO: search failed handling
				cache.SetAsync(keyword, cached);
			}

			return new SearchResultDTO
			{
				KeyWord = keyword,
				Results = cached.Results,
			};
		}
	}
}
