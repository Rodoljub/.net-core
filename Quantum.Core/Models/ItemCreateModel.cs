using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models
{
    public class ItemCreateModel
    {
        //[Required]
        //[MinLength(4, ErrorMessage = "The Title should contain at least 4 characters."),
        // MaxLength(25, ErrorMessage = "The Title should contain max 25 characters.")]
        //public string Title { get; set; }

        //[Required]
        //public string[] Tags { get; set; }

        //[Required]
        [MaxLength(250, ErrorMessage = "The Description should contain max 250 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select the valid file")]
		public IFormFile File { get; set; }
	}
}
