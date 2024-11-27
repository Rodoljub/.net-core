using Quantum.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quantum.Data.Entities
{
    public class FileType :  BaseEntity
    {
		[MaxLength(255)]
		public string Name { get; set; }

		public virtual ICollection<File> Files { get; set; }

	}
}
