using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
    public class ResetPasswordTokenModel
    {
		public string UserId { get; set; }

		public string Token { get; set; }
	}
}
