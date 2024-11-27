using Quantum.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantum.Data.Entities
{
    public class Tag : BaseEntity
	{
		[Required]
		[MaxLength(100)]
		[Column(Order = 1)]
		public string Name { get; set; }

		public string ResourceKey { get; set; }

		public ICollection<ItemTag> ItemTags { get; set; }

        public virtual ICollection<SaveSearchTags> SaveSearchTags { get; set; }

    }
}
