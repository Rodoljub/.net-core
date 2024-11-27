using System.ComponentModel.DataAnnotations;

namespace Quantum.Core.Models.Auth
{
    public class EmailModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }
    }
}
