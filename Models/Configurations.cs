namespace ApiServer.Models
{
	public class SearchEngineApiSettings
	{
		public GoogleCustomSearchApiSettings googleCustomSearchApiSettings { get; set; }
	}

	/// <summary>
	/// Settings for google custom search json api module.
	/// </summary>
	public class GoogleCustomSearchApiSettings
	{
		/// <summary>
		/// api 키
		/// </summary>
		public string ApiKey { get; set; }
		/// <summary>
		/// 검색 엔진 id
		/// </summary>
		public string Cx { get; set; }
	}

	public class RedisSettings
	{
		/// <summary>
		/// Redis Host
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Redis Port
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Prefix automatically added to any key written to the cache
		/// </summary>
		public string InstanceName { get; set; }
	}
}
