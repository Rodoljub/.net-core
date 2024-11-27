using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Data.Models
{
    public class ItemTagsCountModel
    {
		public string ItemId { get; set; }

		public int MatchingTagsCount { get; set; }
	}
}
