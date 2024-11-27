using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Integration.Internal.Models
{
	public class SaveImageFileModel
	{
		public string ID { get; set; }

		public string Base64Image { get; set; }

		public string Extension { get; set; }

		public string Type { get; set; }

		public int Width { get; set; }
	}
}
