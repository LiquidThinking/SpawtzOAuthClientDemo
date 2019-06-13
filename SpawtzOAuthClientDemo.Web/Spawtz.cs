using System;
using SpawtzOAuthClientDemo.Web.Controllers;

namespace SpawtzOAuthClientDemo.Web
{
	public static class Spawtz
	{
		public static string CreateLink( string path )
		{
			return new Uri(
				baseUri: new Uri( Config.SpawtzBaseUrl ),
				relativeUri: new Uri( path, UriKind.Relative )
			).ToString();
		}
	}
}