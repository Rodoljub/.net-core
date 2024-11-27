using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantum.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantum.Data.Entities
{
	public class Item : BaseEntity
	{


		[MaxLength(100)]
		public string Title { get; set; }

		[MaxLength(250)]
		public string Description { get; set; }


		[ForeignKey("File")]
		public string FileID { get; set; }
		public virtual File File { get; set; }

		public virtual ICollection<ItemTag> ItemTags { get; set; }

        public virtual ICollection<Like> Likes { get; set; }

        public virtual ICollection<Favourite> Favourites { get; set; }

        [ForeignKey("ParentId")]
		public virtual ICollection<Comment> Comments { get; set; }

		[ForeignKey("UserProfile")]
		public string UserProfileId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual UserProfile UserProfile { get; set; }

		public int CommentsCount { get; set; }

		public int LikesCount { get; set; }

		public int FavouritesCount { get; set; }

		public bool EnableComments { get; set; } = true;
    }

	internal class ItemConfiguration : BaseEntityTypeConfiguration<Item>
	{
		public override void Configure(EntityTypeBuilder<Item> builder)
		{
			base.Configure(builder);

		}
	}
}