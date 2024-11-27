using Quantum.Data.Models.ReadModels;
using Quantum.Utility.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models
{
    public class CategoryModel
    {
		public string Name { get; set; }

		public ItemViewModel ItemViewModel { get; set; }
	}
}
