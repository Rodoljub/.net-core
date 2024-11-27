using Quantum.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quantum.Data.Entities
{
    public class CLR_Type : BaseEntity
    {
		[MaxLength(255)]
		public string Name { get; set; }

		public virtual ICollection<Comment> Comments { get; set; }

		public virtual ICollection<Like> Likes { get; set; }

		public virtual ICollection<ReportedContent> ReportedContents { get; set; }

		public virtual ICollection<ReportedContentReason> ReportedContentReasons { get; set; }
	}
}
