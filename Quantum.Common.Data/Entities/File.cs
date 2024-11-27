using Quantum.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantum.Data.Entities
{
    public class File : BaseEntity
    {
		[MaxLength(255)]
		public string Name { get; set; }

		[Column("File")]
		public byte[] file { get; set; }

		[ForeignKey("FileType")]
		public string TypeId { get; set; }
		public virtual FileType	FileType { get; set; }

		[ForeignKey("Folder")]
		public string FolderId { get; set; }

		public virtual Folder Folder { get; set; }

		public string Extension { get; set; }

		[ForeignKey("FileDetails")]
		public string FileDetailsId { get; set; }

		public virtual FileDetails FileDetails { get; set; }

		public virtual Item Item { get; set; }

	}
}
