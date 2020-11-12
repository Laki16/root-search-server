using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Google.Apis.Customsearch.v1;
using ApiServer.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

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

		private readonly IReadOnlyList<string> thumbnailKeyCandidates = new List<string>
		{
			"thumbnail",
			"cse_thumbnail",
			"cse_image"
		}.AsReadOnly();

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
			}
			catch (Exception e)
			{
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

			Console.WriteLine($"Search... {keyword}");

			var items = listRequest.Execute().Items;

			var results = new List<SearchResultObject>();

			foreach (var item in items)
			{
				string thumbnail = null;

				if (item.Pagemap != null)
				{
					// JObject 에서 썸네일 추출
					foreach (var key in thumbnailKeyCandidates)
					{
						// 이미 썸네일을 찾았다면 탈출
						if (!string.IsNullOrEmpty(thumbnail))
						{
							break;
						}

						// 후보키를 JObject에서 찾을 수 없다면 스킵
						if (!item.Pagemap.TryGetValue(key, out var jArrayObject))
						{
							continue;
						}

						var jsonArray = JArray.FromObject(jArrayObject);
						foreach (var jsonObj in jsonArray)
						{
							dynamic obj = jsonObj;
							string src = obj.src;

							if (src == null)
							{
								continue;
							}

							// 찾은 썸네일 저장하고 탈출
							thumbnail = src;
							break;
						}
					}
				}

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
