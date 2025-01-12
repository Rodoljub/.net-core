﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
    public class EmailModel
    {
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		public string ReturnUrl { get; set; }
	}
}
