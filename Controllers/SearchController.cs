using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using ApiServer.Models;

namespace ApiServer.Controllers
{
	[ApiController]
	[Route("search")]
	public class SearchController : ControllerBase
	{
		public SearchController(IOptions<SearchEngineApiSettings> apiSettingsAccessor, ICacheManager cache)
		{
			// init worker manager with dependencies injection.
			WorkerManager.Instance.Initialize(apiSettingsAccessor.Value, cache);
		}

		/// <summary>
		/// Get result of search by {keyword}
		/// </summary>
		/// <param name="keyword">search target</param>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<SearchResultDTO>> GetSearchResult(string keyword)
		{
			var results = await WorkerManager.Instance.GetResult(keyword);

			return new SearchResultDTO
			{
				KeyWord = keyword,
				Results = results,
			};
		}
	}
}
