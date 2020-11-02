namespace ApiServer.Models
{
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

	public class SearchResultDTO
	{
		/// <summary>
		/// 검색 키워드
		/// </summary>
		public string KeyWord { get; set; }

		/// <summary>
		/// 검색 결과
		/// </summary>
		public string[] Results { get; set; }
	}
}
