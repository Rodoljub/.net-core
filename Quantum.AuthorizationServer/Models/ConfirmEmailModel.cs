using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
    public class ConfirmEmailModel
    {
		[Required]
		public string userId { get; set; }

		[Required]
		public string token { get; set; }

		public string ReturnUrl { get; set; }
	}
}
