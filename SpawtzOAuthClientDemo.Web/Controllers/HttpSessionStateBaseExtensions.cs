using System.Web;

namespace SpawtzOAuthClientDemo.Web.Controllers
{
	public static class HttpSessionStateBaseExtensions
	{
		public static SpawtzPermissions GetSpawtzPermissions( this HttpSessionStateBase session )
		{
			return session[ nameof( SpawtzPermissions ) ] as SpawtzPermissions;
		}

		public static void SetSpawtzPermissions( this HttpSessionStateBase session, SpawtzPermissions spawtzPermissions )
		{
			session[ nameof( SpawtzPermissions ) ] = spawtzPermissions;
		}
	}
}