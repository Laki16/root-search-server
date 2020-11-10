using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ApiServer.Models;
using System.Text;

namespace ApiServer.Controllers
{
	[EnableCors("DevelopPolicy")]
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
		public async Task GetSearchResult(string keyword)
		{
			Response.Headers.Add("Connection", "keep-alive");
			Response.Headers.Add("Cache-Control", "no-cache");
			Response.Headers.Add("Access-Control-Allow-Origin", "127.0.0.1:3000");
			Response.Headers.Add("Access-Control-Allow-Methods", "GET");
			// Response.Headers.Add("Access-Control-Allow-Credentials", "true");
			Response.Headers.Add("Content-Type", "text/event-stream");
			Response.StatusCode = 200;

			ResultManager.Instance.AddConnection(Request.HttpContext.Connection.Id, keyword, Response);

			try
			{
				while (!Response.HttpContext.RequestAborted.IsCancellationRequested)
				{
					await Task.Delay(1000);
				}
			}
			finally
			{
				Response.Body.Close();
			}

			// return Ok();
			// var result = await WorkerManager.Instance.GetResult(keyword);

			// return new SearchResultDTO
			// {
			// 	KeyWord = keyword,
			// 	Results = result.Results,
			// };
		}
	}
}
