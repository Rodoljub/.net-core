using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models
{
    public class LikeModel
    {
		[Required]
		public string EntityId { get; set; }

		[Required]
		public string Type { get; set; }
	}
}
