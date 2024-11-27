using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantum.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Quantum.Data.Entities
{
    public class Like : BaseEntity
    {
		[Required]
		public string EntityId { get; set; }

		[ForeignKey("ParentType")]
		public string EntityTypeID { get; set; }

		public virtual CLR_Type EntityType { get; set; }


        public string ItemID { get; set; }

        [InverseProperty("Likes")]
        public virtual Item Item { get; set; }

        public string CommentID { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        [InverseProperty("Likes")]
        public virtual Comment Comment { get; set; }
    }

    internal class LikeConfiguration : BaseEntityTypeConfiguration<Like>
    {

        public override void Configure(EntityTypeBuilder<Like> builder)
        {
            base.Configure(builder);

            builder.Ignore(p => p.ID);

            builder.HasKey(p => new { p.EntityId, p.CreatedById });

            builder.HasIndex(p => p.EntityId);

            //builder.HasOne(l => l.Item).WithMany(i => i.Likes)
            //       .HasPrincipalKey(l => l.EntityId);

        }
    }
}
