using System.ComponentModel.DataAnnotations;

namespace Quantum.Core.Models.Auth
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
