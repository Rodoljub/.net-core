using Quantum.Data.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantum.Data.Entities
{
    public class ItemTag : BaseEntity
    {
		public virtual Item Item { get; set; }

		[ForeignKey("Item")]
		public string ItemId { get; set; }

		public virtual Tag Tag { get; set; }

		[ForeignKey("Tag")]
		public string TagID { get; set; }

		public bool Display { get; set; }
	}
}
