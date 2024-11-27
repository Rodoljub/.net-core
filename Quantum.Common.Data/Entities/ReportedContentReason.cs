using Quantum.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantum.Data.Entities
{
    public class ReportedContentReason : BaseEntity
    {
		[MaxLength(600)]
		public string Description { get; set; }

		public string ResourceKey { get; set; }

		[ForeignKey("ReportedContentType")]
		public string ReportedContentTypeID { get; set; }

		public virtual CLR_Type ReportedContentType { get; set; } 

		public virtual ICollection<ReportedContent> ReportedContents { get; set; }
	}
}
