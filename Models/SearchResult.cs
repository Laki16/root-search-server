using System;
using System.Collections.Generic;

namespace ApiServer.Models
{
	/// <summary>
	/// 검색 결과 Unit
	/// </summary>
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
	/// 실제 클라이언트에 전송될 때 추가적인 정보를 포함하여 전송하기 위한 래퍼
	/// </summary>
	public struct SearchResultObjectWrapper
	{
		public SearchResultObjectWrapper(int SeqId, SearchResultObject searchResultObject)
		{
			this.SeqId = SeqId;
			this.Title = searchResultObject.Title;
			this.Snippet = searchResultObject.Snippet;
			this.Link = searchResultObject.Link;
			this.Thumbnail = searchResultObject.Thumbnail;
		}

		/// <summary>
		/// 각 result의 시퀀스 아이디
		/// </summary>
		public int SeqId;

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
		/// 부적절한 연관 검색어
		/// </summary>
		public Dictionary<string, DateTime> BlockedWords { get; set; }

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
		public List<SearchResultObjectWrapper> Results { get; set; }
	}

	/// <summary>
	/// 부적절한 연관 검색어 클래스
	/// </summary>
	public class InappropriateKeyword
	{
		/// <summary>
		/// 검색한 키워드
		/// </summary>
		public string SearchKeyword { get; set; }

		/// <summary>
		/// 부적절한 연관 검색어
		/// </summary>
		/// <value></value>
		public string BlockKeyword { get; set; }
	}
}
