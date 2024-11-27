using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Data.Models
{
    public class SearchRequestModel
    {
		public IEnumerable<string> QueryCollection { get; set; }

		public IEnumerable<string> Tags { get; set; }
	}
}
