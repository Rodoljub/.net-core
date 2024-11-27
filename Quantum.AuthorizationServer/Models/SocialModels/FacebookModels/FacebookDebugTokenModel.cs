using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
    public class FacebookDebugTokenModel
    {
		[JsonProperty("data")]
		public DebugTokenDataModel Data { get; set; }
	}

	public class DebugTokenDataModel
	{
		[JsonProperty("app_id")]
		public string AppId { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("application")]
		public string Application { get; set; }

		[JsonProperty("expires_at")]
		public string Expires_at { get; set; }

		[JsonProperty("is_valid")]
		public string Is_valid { get; set; }

		[JsonProperty("issued_at")]
		public string Issued_at { get; set; }

		[JsonProperty("scopes")]
		public string[] Scopes { get; set; }

		[JsonProperty("user_id")]
		public string User_id { get; set; }

	}
}
