using System.Collections.Generic;

namespace ApiServer.Models
{
	public struct SearchResultObject
	{
		/// <summary>
		/// 제목
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 부가 내용
		/// </summary>
		public string Snippet { get; set; }
		/// <summary>
		/// URL 링크
		/// </summary>
		public string Link { get; set; }
		/// <summary>
		/// 썸네일용 이미지 src
		/// </summary>
		public string Thumbnail { get; set; }
	}

	/// <summary>
	/// 검색 결과 아이템
	/// </summary>
	public class SearchResult
	{
		/// <summary>
		/// 검색 키워드
		/// </summary>
		public string KeyWord { get; set; }

		/// <summary>
		/// 검색 결과
		/// </summary>
		public string[] Results { get; set; }

		/// <summary>
		/// 세션 키
		/// </summary>
		public string SessionKey { get; set; }
	}

	public class SearchResultCache
	{
		/// <summary>
		/// 연관 검색어
		/// </summary>
		public List<string> AssociativeWords { get; set; }

		/// <summary>
		/// 검색 결과
		/// </summary>
		public List<SearchResultObject> Results { get; set; }
	}

	public class SearchResultDTO
	{
		/// <summary>
		/// 검색 키워드
		/// </summary>
		public string KeyWord { get; set; }

		/// <summary>
		/// 검색 결과
		/// </summary>
		public List<SearchResultObject> Results { get; set; }
	}
}
