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
		/// </summary>
		/// <param name="keyword">search target</param>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<SearchResultDTO>> GetSearchResult(string keyword)
		{
			var results = SearchManager.Instance.Search(keyword);

			return new SearchResultDTO{
				KeyWord = keyword,
				Results = results,
			};
		}
	}
}
