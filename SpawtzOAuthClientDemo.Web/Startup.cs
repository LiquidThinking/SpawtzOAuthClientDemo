using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute( typeof( SpawtzOAuthClientDemo.Web.Startup ) )]

namespace SpawtzOAuthClientDemo.Web
{
	public partial class Startup
	{
		public void Configuration( IAppBuilder app )
		{
		}
	}
}