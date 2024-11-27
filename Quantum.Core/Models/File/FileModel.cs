using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models.File
{
    public class FileModel
    {
		[Required]
		public IFormFile File { get; set; }

		public string FolderId { get; set; }

		public string FileTypeId { get; set; }
	}
}
