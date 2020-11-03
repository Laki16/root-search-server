using System;
using System.Collections.Generic;
using ApiServer.Models;

namespace ApiServer
{
	public class SearchManager
	{
		private static SearchManager _instance = new SearchManager();

		public static SearchManager Instance => _instance;

		/// <summary>
		/// Api settings for search modules
		/// </summary>
		private SearchEngineApiSettings _apiSettings;

		/// <summary>
		/// List of available search modules
		/// </summary>
		private List<ISearchModule> _searchModules = new List<ISearchModule>();

		public void Initialize(SearchEngineApiSettings apiSettings)
		{
			_apiSettings = apiSettings;

			TryAddSearchModule(GoogleCustomSearchModule.Instance);
		}

		/// <summary>
		/// Try to activate search module and add to available search module list.
		/// </summary>
		/// <param name="searchModule">target search module</param>
		/// <returns>true if successfully added</returns>
		private bool TryAddSearchModule(ISearchModule searchModule)
		{
			// error: already added
			if (_searchModules.Contains(searchModule))
			{
				return false;
			}

			// error: couldn't initialize
			if (!searchModule.Initialize(_apiSettings))
			{
				return false;
			}

			_searchModules.Add(searchModule);

			return true;
		}

		private ISearchModule DecideSearchModule()
		{
			if (_searchModules == null || _searchModules.Count == 0) {
				return null;
			}

			for (int i = _searchModules.Count - 1; i >= 0; --i)
			{
				if (!_searchModules[i].IsAvailable)
				{
					continue;
				}

				return _searchModules[i];
			}

			return null;
		}

		public List<SearchResultObject> Search(string keyword)
		{
			// TODO: handling for not found available module
			var module = DecideSearchModule();

			return module?.Search(keyword);
		}
	}
}
