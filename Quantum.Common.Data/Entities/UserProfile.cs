using Quantum.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantum.Data.Entities
{
    public class UserProfile : BaseEntity
	{
		[MaxLength(25)]
		public string Name { get; set; }

		public string UrlSegment { get; set; }

        [MaxLength(255)]
		public string Email { get; set; }

		public string ImageFileId { get; set; }

		[ForeignKey("File")]
		public string FileId { get; set; }
		public File File { get; set; }

        public bool TermsAndConditions { get; set; }

        public int UploadsCount { get; set; }

		public int CommentsCount { get; set; }

		public int LikesCount { get; set; }

		public int FavouritesCount { get; set; }

        public virtual ICollection<Item> Items { get; set; }

		public virtual ICollection<Comment> Comments { get; set; }
	}
}
