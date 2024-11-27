using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
    public class UserFacebookInformationModel
    {
		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("returnUrl")]
		public string ReturnUrl { get; set; }
	}
}
