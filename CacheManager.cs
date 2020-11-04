using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using ApiServer.Models;

namespace ApiServer
{
	public class CacheManager : ICacheManager
	{
		private readonly IDistributedCache _cache;

		public CacheManager(IDistributedCache distributedCache)
		{
			_cache = distributedCache;
		}

		public async Task<List<SearchResultObject>> GetAsync(string key)
		{
			return await _cache.GetAsync<List<SearchResultObject>>(key);
		}

		public async void SetAsync(string key, List<SearchResultObject> results)
		{
			await _cache.SetAsync<List<SearchResultObject>>(key, results);
		}
	}
}
