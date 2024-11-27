using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantum.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Quantum.Data.Entities
{
    [Index(nameof(NotificationId))]
    public class UserNotification : BaseEntity
    {
        public string NotificationType { get; set; }

        public string NotificationId { get; set; }

        [ForeignKey("Item")]
        public string ItemID { get; set; }

        public virtual Item Item { get; set; }

        public string Status { get; set; }

        public string Value { get; set; }

    }

    internal class UserNotificationConfiguration : BaseEntityTypeConfiguration<UserNotification>
    {
        public override void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            base.Configure(builder);


        //    builder.Property(e => e.Value)
        //           .HasConversion(
        //         v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
        //v => JsonSerializer.Deserialize<Object>(v, (JsonSerializerOptions)null));

        }
    }
}
