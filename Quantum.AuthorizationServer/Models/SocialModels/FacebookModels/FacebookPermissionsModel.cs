using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models.SocialModels.FacebookModels
{
    public class FacebookPermissionsModel
    {
		[JsonProperty("data")]
		public Permissions[] Permissions { get; set; }
	}

	public class Permissions
	{
		[JsonProperty("permission")]
		public string Permission { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }
	}
}
