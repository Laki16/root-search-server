using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using ApiServer.Models;

namespace ApiServer
{
	public class RedisCacheModule : ICacheModule
	{
		private readonly IDistributedCache _cache;

		public RedisCacheModule(IDistributedCache distributedCache)
		{
			_cache = distributedCache;
		}

		public async Task<SearchResultCache> GetAsync(string key)
		{
			return await _cache.GetAsync<SearchResultCache>(key);
		}

		public async Task SetAsync(string key, SearchResultCache result)
		{
			await _cache.SetAsync<SearchResultCache>(key, result);
		}

		public async Task RemoveAsync(string key)
		{
			await _cache.RemoveAsync(key);
		}
	}
}
