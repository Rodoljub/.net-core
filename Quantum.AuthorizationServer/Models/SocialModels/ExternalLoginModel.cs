using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
    public class ExternalLoginModel
    {

		[JsonProperty("provider")]
		public string Provider { get; set; }

		[JsonProperty("returnUrl")]
		public string ReturnUrl { get; set; }

		[JsonProperty("localUrl")]
		public string LocalUrl { get; set; }

		[JsonProperty("keepMeLoggedIn")]
		public bool KeepMeLoggedIn { get; set; }
	}
}
