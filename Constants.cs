using System;

namespace ApiServer
{
	public static class Constants
	{
		/// <summary>
		/// 추출할 연관 검색어 최대 개수
		/// </summary>
		public static int MaxAssociativeWords = 5;

		/// <summary>
		/// 블럭할 부적절한 연관 검색어 최대 개수
		/// </summary>
		public static int MaxBlockedWords = 4;

		/// <summary>
		/// 블럭한 연관검색어가 만료되는 시각 (1일)
		/// </summary>
		public static TimeSpan blockExpireAfter = new TimeSpan(24, 0, 0);
	}
}
