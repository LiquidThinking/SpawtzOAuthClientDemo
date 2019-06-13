using System.Collections.Generic;

namespace SpawtzOAuthClientDemo.Web.Controllers
{
	public class SpawtzPermissions
	{
		public string Role { get; set; }
		public string RoleDescription { get; set; }
		public string Level { get; set; }
		public Dictionary<string, List<string>> Permissions { get; set; }
		public List<(int Id, string Name)> SecurityRoleAssociatedItems { get; set; }
	}
}