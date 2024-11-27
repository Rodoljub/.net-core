using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models
{
    public class CommentModel
    {
		[Required]
		public string ParentId { get; set; }

		[Required]
		public string typeName { get; set; }

		[Required]
		public string Content { get; set; }
	}
}
