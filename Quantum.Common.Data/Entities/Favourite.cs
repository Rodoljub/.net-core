using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantum.Data.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantum.Data.Entities
{
    public class Favourite : BaseEntity
    {
		[Required]
		public string EntityId { get; set; }

		[ForeignKey("ParentType")]
		public string EntityTypeID { get; set; }

		public virtual CLR_Type EntityType { get; set; }

		[Range(1, int.MaxValue)]
		public int OrderNumber { get; set; }

        public string ItemID { get; set; }

        [InverseProperty("Favourites")]
        public virtual Item Item { get; set; }
    }


    internal class FavouriteConfiguration : BaseEntityTypeConfiguration<Favourite>
    {
        public override void Configure(EntityTypeBuilder<Favourite> builder)
        {
            base.Configure(builder);

            builder.Ignore(p => p.ID);

            builder.HasKey(p => new { p.EntityId, p.CreatedById });

        }
    }
}
