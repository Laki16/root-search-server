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

		private bool _isAvailable = true;

		public bool IsAvailable
		{
			get => _isAvailable;
			set => _isAvailable = value;
		}

		private const string apiKey = "APIKEY";

		private const string cx = "CX";

		private CustomsearchService customSearchService =
			new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer()
			{
				ApiKey = apiKey
			});

		public void Search(string keyword)
		{
			CseResource.ListRequest listRequest = customSearchService.Cse.List();

			listRequest.Q = keyword;
			listRequest.Cx = cx;

			Console.Write($"Start searching... {keyword}");

			var results = listRequest.Execute().Items;
		}
	}
}
