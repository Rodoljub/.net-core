using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
	[DataContract]
	[Newtonsoft.Json.JsonObject(MemberSerialization = Newtonsoft.Json.MemberSerialization.OptIn)]
	public class ChangePasswordModel
    {
		[Required]
		[DataMember(Order = 1), Newtonsoft.Json.JsonProperty]
		public string CurrentPassword { get; set; }

		[Required]
		[DataMember(Order = 2), Newtonsoft.Json.JsonProperty]
		public string Password { get; set; }

		[Required]
		[DataMember(Order = 3), Newtonsoft.Json.JsonProperty]
		[Compare("Password", ErrorMessage = "Passwords doesnt match!")]
		public string ConfirmPassword { get; set; }

	}
}
