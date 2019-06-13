using System.Configuration;

namespace SpawtzOAuthClientDemo.Web
{
	public static class Config
	{
		public static string SpawtzBaseUrl
			=> ConfigurationManager.AppSettings[ nameof( SpawtzBaseUrl ) ];

		public static string ClientId
			=> ConfigurationManager.AppSettings[ nameof( ClientId ) ];

		public static string ClientSecret
			=> ConfigurationManager.AppSettings[ nameof( ClientSecret ) ];
	}
}