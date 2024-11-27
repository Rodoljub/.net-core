using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantum.Data.Entities
{
    public class ReportedContent : BaseEntity
    {
		[Required]
		public string ReportedId { get; set; }

		[ForeignKey("ReportedType")]
		public string ReportedTypeID { get; set; }

		public virtual CLR_Type ReportedType { get; set; }

		[ForeignKey("ReportedBy")]
		public string ReporterId { get; set; }

		public virtual IdentityUser ReportedBy { get; set; }

		[ForeignKey("ReportedContentReason")]
		public string ReportedContentReasonId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual ReportedContentReason ReportedContentReason { get; set; }
	}
}
