using Quantum.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quantum.Data.Entities
{
    public class Folder : BaseEntity
    {
		[MaxLength(255)]
		public string Name { get; set; }

		public string ParentId { get; set; }

		public virtual ICollection<File> Files { get; set; }

	}
}
