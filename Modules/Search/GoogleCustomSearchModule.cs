using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Google.Apis.Customsearch.v1;
using ApiServer.Models;

namespace ApiServer
{
	public class GoogleCustomSearchModule : ISearchModule
	{
		private static GoogleCustomSearchModule _instance = new GoogleCustomSearchModule();

		public static GoogleCustomSearchModule Instance => _instance;

		public SearchEngineTypes SearchEngingType => SearchEngineTypes.GoogleCustomJson;

		private bool _isAvailable = false;

		public bool IsAvailable
		{
			get => _isAvailable;
			set => _isAvailable = value;
		}

		private string cx;

		private CustomsearchService customSearchService;

		public bool Initialize(SearchEngineApiSettings apiSettings)
		{
			try
			{
				customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer()
				{
					ApiKey = apiSettings.googleCustomSearchApiSettings.ApiKey
				});

				cx = apiSettings.googleCustomSearchApiSettings.Cx;

				_isAvailable = true;

				return true;
			} catch (Exception e) {
				_isAvailable = false;

				return false;
			}
		}

		/// <summary>
		/// 검색 결과를 리스트로 반환
		/// TODO: 리스트 풀링하도록 수정
		/// </summary>
		/// <param name="keyword">검색할 키워드</param>
		public List<SearchResultObject> Search(string keyword)
		{
			CseResource.ListRequest listRequest = customSearchService.Cse.List();

			listRequest.Q = keyword;
			listRequest.Cx = cx;

			Console.Write($"Start searching... {keyword}");

			var items = listRequest.Execute().Items;

			var results = new List<SearchResultObject>();

			foreach (var item in items)
			{
				string thumbnail = null;

				// if (item.Pagemap.TryGetValue("cse_thumbnail", out var cse))
				// {
				// 	thumbnail = (string)cse.GetType().GetProperty("src").GetValue(cse);
				// }

				results.Add(new SearchResultObject
				{
					Title = item.Title,
					Snippet = item.Snippet,
					Link = item.Link,
					Thumbnail = thumbnail
				});
			}

			return results;
		}
	}
}
