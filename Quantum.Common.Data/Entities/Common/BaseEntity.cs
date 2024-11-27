
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Quantum.Data.Entities.Common
{
	public class BaseEntity
	{
		[Key]
		[MaxLength(32)]
		public string ID { get; set; }

        public DateTime CreatedDate { get; protected internal set; } = default;

		public DateTime LastModified { get; protected internal set; } = default;

        public DateTime DeletedOn { get; internal protected set; } = default;

        public bool IsDeleted { get; internal protected set; }

        //[DeleteBehavior(DeleteBehavior.NoAction)]
        [ForeignKey("CreatedBy")]
        public string CreatedById { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public IdentityUser? CreatedBy { get; set; }

        //[DeleteBehavior(DeleteBehavior.NoAction)]
        [ForeignKey("LastModifiedBy")]
        public string LastModifiedById { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public IdentityUser? LastModifiedBy { get; set; }

        //[DeleteBehavior(DeleteBehavior.NoAction)]
        [ForeignKey("DeletedBy")]
        public string DeletedById { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public IdentityUser? DeletedBy { get; set; }
    }


    internal class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            
        }

    }
}
