using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Quantum.Data.Entities
{
    public class Comment : BaseEntity
    {
		[Required]
		[MaxLength(2000)]
		public string Content { get; set; }

		public virtual Item Item { get; set; }

		[Required]
		public string ParentId { get; set; }


		[ForeignKey("ParentType")]
		public string ParentTypeID { get; set; }

		public virtual CLR_Type ParentType { get; set; }

		[ForeignKey("UserProfile")]
		public string UserProfileId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual UserProfile UserProfile { get; set; }

		public virtual ICollection<Like> Likes { get; set; }

		public int ChildCount { get; set; } = 0;

		public int LikeCount { get; set; }
	}
}
