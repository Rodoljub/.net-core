using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models.Folder
{
    public class FolderModel
    {
		[Required]
		public string Name { get; set; }

		public string ParentId { get; set; }
	}
}
