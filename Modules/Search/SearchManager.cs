using System;
using System.Collections.Generic;

namespace ApiServer
{
	public class SearchManager
	{
		private static SearchManager _instance = new SearchManager();

		public static SearchManager Instance => _instance;

		private bool _isInitialized = false;

		private List<ISearchModule> _searchModules = new List<ISearchModule>();

		public void Initialize()
		{
			if (_isInitialized)
			{
				return;
			}

			_searchModules.Add(GoogleCustomSearchModule.Instance);

			_isInitialized = true;
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

		public void Search(string keyword)
		{
			var module = DecideSearchModule();

			module?.Search(keyword);
		}
	}
}
