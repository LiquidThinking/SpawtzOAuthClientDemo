using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace SpawtzOAuthClientDemo.Web.Controllers
{
	public class SpawtzLoginRedirectorController : Controller
	{
		[HttpGet]
		[Route( "SpawtzAuthorise" )]
		public ActionResult Index( string redirectUrl = null )
		{
			var authorizeArgs = new Dictionary<string, string>
			{
				{ "client_id", Config.ClientId },
				{ "scope", string.Empty },
				{ "response_type", "code" },
				{ "redirect_uri", HttpUtility.UrlEncode( CreateRedirect() ) },
				{ "state", Base64Encoder.Base64Encode( redirectUrl ?? string.Empty ) }
			};

			var oauthAuthorisePath = "/oauth/authorize?"
									+ string
										.Join( "&", authorizeArgs.Select( x => $"{x.Key}={x.Value}" ) );

			return Redirect( Spawtz.CreateLink( oauthAuthorisePath ) );
		}

		public async Task<ActionResult> AuthorizationCodeCallback()
		{
			var codes = Request.Params.GetValues( "code" );
			var authorizationCode = "";
			if ( codes?.Length > 0 )
				authorizationCode = codes[ 0 ];

			var json = await ConvertCodeToTokenAsync( authorizationCode );
			var accessToken = json[ "access_token" ].ToString();

			using ( var client = new HttpClient() )
			{
				var userJsonRequest = CreateBearerRequest(
					Spawtz.CreateLink( "/currentclientinformation" ),
					accessToken );

				var userJson = await ( await client.SendAsync( userJsonRequest ) )
					.Content
					.ReadAsStringAsync();

				var @id = JObject
					.Parse( userJson )[ "AuthenticatedUserId" ]
					.Value<int>()
					.ToString();

				var roleJson = await ( await client
						.SendAsync( CreateBearerRequest(
							Spawtz.CreateLink( $"/api/v1/Users/{@id}/SecurityRole" ),
							accessToken ) ) )
					.Content
					.ReadAsStringAsync();

				var permissionsJson = await ( await client
						.SendAsync( CreateBearerRequest(
							Spawtz.CreateLink( $"/api/v1/Users/{@id}" ),
							accessToken ) ) )
					.Content
					.ReadAsStringAsync();

				var role = JObject.Parse( roleJson );
				var securityRoleAssociatedItems = (JArray)JObject
					.Parse( permissionsJson )[ "SecurityRoleAssociatedItems" ];

				var spawtzPermissions = new SpawtzPermissions
				{
					Role = role[ "Name" ].Value<string>(),
					RoleDescription = role[ "Description" ].Value<string>(),
					Level = role[ "AssociationLevel" ][ "Value" ].Value<string>(),
					Permissions = role[ "Permissions" ].ToObject<Dictionary<string, List<string>>>(),
					SecurityRoleAssociatedItems = securityRoleAssociatedItems
						.Select( x => ( x[ "Id" ].Value<int>(), x[ "Value" ].Value<string>())  )
						.ToList()
				};

				HttpContext
					.Session
					.SetSpawtzPermissions( spawtzPermissions );
			}

			var encodedRedirect = Request.Params.Get( "state" );
			var redirect = Base64Encoder.Base64Decode( encodedRedirect );
			if ( string.IsNullOrEmpty( redirect ) )
				return RedirectToAction( "Index", "Home" );

			return Redirect( redirect );
		}

		private static HttpRequestMessage CreateBearerRequest( string endpoint, string bearerToken )
		{
			var request = new HttpRequestMessage( HttpMethod.Get, endpoint );
			request
				.Headers
				.Authorization = new AuthenticationHeaderValue( "Bearer", bearerToken );

			request
				.Headers
				.Accept
				.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );

			return request;
		}

		private async Task<JObject> ConvertCodeToTokenAsync( string code )
		{
			var post = new Dictionary<string, string>
			{
				{ "client_id", Config.ClientId },
				{ "redirect_uri", CreateRedirect() },
				{ "client_secret", Config.ClientSecret },
				{ "code", code },
				{ "grant_type", "authorization_code" }
			};

			using ( var client = new HttpClient() )
			{
				client.DefaultRequestHeaders.Add( "Accept", "application/json" );
				var response = await client
					.PostAsync(
						Spawtz.CreateLink( "/oauth/token" ),
						new FormUrlEncodedContent( post )
					);

				var content = await response
					.Content
					.ReadAsStringAsync();

				return JObject.Parse( content );
			}
		}

		private string CreateRedirect()
		{
			return Url.Action(
				nameof( AuthorizationCodeCallback ),
				"SpawtzLoginRedirector",
				null,
				Request.Url.Scheme
			);
		}
	}
}