using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Quantum.Utility.Filters
{
	public class RequiredStringCollectionAttribute : ValidationAttribute
	{
		public int MinimumElements { get; set; } = 1;

		public override bool IsValid(object value)
		{
			var list = value as IEnumerable<string>;
			if (list != null)
			{
				return list.Count() >= MinimumElements;
			}
			return false;
		}
	}
}
