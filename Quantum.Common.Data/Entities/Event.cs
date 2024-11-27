using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantum.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Quantum.Data.Entities
{
    public class Event : BaseEntity
    {
        public string EventType { get; set; }

        public string EntityId { get; set; }

        public string Status { get; set; }

        public int NoOfFails { get; set; } = 0;

        public string Value { get; set; }

    }

    internal class EventConfiguration : BaseEntityTypeConfiguration<Event>
    {
        public override void Configure(EntityTypeBuilder<Event> builder)
        {
            base.Configure(builder);

            //builder.Property(e => e.NoOfFails)
            //       .HasDefaultValue(0);

        //    builder.Property(e => e.Value)
        //           .HasConversion(
        //         v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
        //v => JsonSerializer.Deserialize<Object>(v, (JsonSerializerOptions)null));

        }
    }
}
