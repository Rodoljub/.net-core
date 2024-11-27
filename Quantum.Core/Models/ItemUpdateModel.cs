using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models
{
	public class ItemUpdateModel
	{
		//[Required]
		//[MinLength(4, ErrorMessage = "The Title should contain at least 4 characters."),
		 [MaxLength(100, ErrorMessage = "The Title should contain max 70 characters.")]
		public string Title { get; set; }

		[Required]
		public string[] Tags { get; set; }

        //[Required]
        //[MinLength(4, ErrorMessage = "The Description should contain at least 4 characters."),
        [MaxLength(250, ErrorMessage = "The Description should contain max 250 characters.")]
        public string Description { get; set; }

		public bool EnableComments { get; set; } = true;

        [Required]
		public string ItemId { get; set; }
	}
}
