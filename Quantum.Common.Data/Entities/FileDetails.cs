using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantum.Data.Entities.Common;
using System;
using System.Text.Json;

namespace Quantum.Data.Entities
{
    public class FileDetails : BaseEntity
	{
		public double Width { get; set; }

		public double Height { get; set; }

		public string Color { get; set; }

		public string ImageAnalysis { get; set; }
	}

    internal class FileDetailsConfiguration : BaseEntityTypeConfiguration<FileDetails>
    {
        public override void Configure(EntityTypeBuilder<FileDetails> builder)
        {
            base.Configure(builder);


        //    builder.Property(e => e.ImageAnalysis)
        ////           .HasConversion(
        //         v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
        //v => JsonSerializer.Deserialize<Object>(v, (JsonSerializerOptions)null));

        }
    }
}
