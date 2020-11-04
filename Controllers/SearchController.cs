using System;
using System.Collections.Generic;
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

		public SearchController(IOptions<SearchEngineApiSettings> apiSettingsAccessor)
		{
			// inject IOptions
			_apiSettings = apiSettingsAccessor.Value;

			SearchManager.Instance.Initialize(_apiSettings);
		}

		/// <summary>
		/// Get result of search by {keyword}
		///
		/// injected cache dependency via FromServices attribute.
		/// </summary>
		/// <param name="keyword">search target</param>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<SearchResultDTO>> GetSearchResult([FromServices] ICacheManager cache, string keyword)
		{
			List<SearchResultObject> results = await cache.GetAsync(keyword);

			if (results == null)
			{
				results = SearchManager.Instance.Search(keyword);

				// TODO: search failed handling
				cache.SetAsync(keyword, results);
			}

			return new SearchResultDTO
			{
				KeyWord = keyword,
				Results = results,
			};
		}
	}
}
