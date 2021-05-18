using System.Collections.Generic;
using System.Threading.Tasks;
using ApiServer.Models;

namespace ApiServer
{
	public interface ICacheManager
	{
		Task<SearchResultCache> GetAsync(string key);

		Task SetAsync(string key, SearchResultCache result);
	}
}
