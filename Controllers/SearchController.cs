using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiServer.Models;

namespace ApiServer.Controllers
{
	[ApiController]
	[Route("search")]
	public class SearchController : ControllerBase
	{
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
