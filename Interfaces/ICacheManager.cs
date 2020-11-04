using System.Collections.Generic;
using System.Threading.Tasks;
using ApiServer.Models;

namespace ApiServer
{
	public interface ICacheManager
	{
		Task<List<SearchResultObject>> GetAsync(string key);

		void SetAsync(string key, List<SearchResultObject> result);
	}
}
