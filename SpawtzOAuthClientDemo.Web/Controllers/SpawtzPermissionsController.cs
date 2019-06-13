using System.Web;
using System.Web.Mvc;

namespace SpawtzOAuthClientDemo.Web.Controllers
{
	public class SpawtzPermissionsController : Controller
	{
		[HttpGet]
		[Route( "SpawtzPermissions" )]
		public ActionResult Index()
		{
			var permissions = HttpContext
				.Session
				.GetSpawtzPermissions();

			return permissions != null
				? (ActionResult)View( permissions )
				: Redirect( $"/SpawtzAuthorise?redirectUrl={HttpUtility.UrlEncode( Request.Url.PathAndQuery )}" );
		}
	}
}