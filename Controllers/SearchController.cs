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
		/// <summary>
		/// api settings for search modules
		/// </summary>
		private readonly SearchEngineApiSettings _apiSettings;

		/// <summary>
		/// redis distributed cache for search results
		/// </summary>
		private readonly ICacheManager _cache;

		public SearchController(IOptions<SearchEngineApiSettings> apiSettingsAccessor, ICacheManager cache)
		{
			// inject IOptions
			_apiSettings = apiSettingsAccessor.Value;

			// inject cache
			_cache = cache;

			WorkerManager.Instance.Initialize(_apiSettings);
		}

		/// <summary>
		/// Get result of search by {keyword}
		/// </summary>
		/// <param name="keyword">search target</param>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<SearchResultDTO>> GetSearchResult(string keyword)
		{
			var results = await WorkerManager.Instance.GetResult(_cache, keyword);

			return new SearchResultDTO
			{
				KeyWord = keyword,
				Results = results,
			};
		}
	}
}
