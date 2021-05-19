using System.Threading.Tasks;
using ApiServer.Models;

namespace ApiServer
{
	public interface ICacheModule
	{
		Task<SearchResultCache> GetAsync(string key);

		Task SetAsync(string key, SearchResultCache result);

		Task RemoveAsync(string key);
	}
}
