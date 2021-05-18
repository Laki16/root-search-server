using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace Microsoft.Extensions.Caching.Distributed
{
	public static class IDistributedCacheExtensions
	{
		public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key) where T : class
		{
			var serializedResult = await cache.GetAsync(key);
			if (serializedResult == null)
				return null;

			var decrypted = Encoding.UTF8.GetString(serializedResult);

			return JsonConvert.DeserializeObject<T>(decrypted);
		}

		public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options = null)
			where T : class
		{
			// decide cache entry options
			var entryOptions = options ??
				new DistributedCacheEntryOptions()
					.SetAbsoluteExpiration(DateTime.Now.AddHours(10))
					.SetSlidingExpiration(TimeSpan.FromHours(5));

			var serializedResult = JsonConvert.SerializeObject(value);

			var encrypted = Encoding.UTF8.GetBytes(serializedResult);

			await cache.SetAsync(key, encrypted, entryOptions);
		}

		public static async Task RemoveAsync(this IDistributedCache cache, string key)
		{
			await cache.RemoveAsync(key);
		}
	}
}
