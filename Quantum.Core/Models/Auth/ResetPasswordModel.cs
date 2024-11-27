using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models.Auth
{
    public class ResetPasswordModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords doesnt match!")]
        public string ConfirmPassword { get; set; }
    }
}
