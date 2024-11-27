using Quantum.Integration.Internal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Integration.Internal.Services.Contracts
{
	public interface IWebServerService
	{
		void SaveImageOnWebServer(List<SaveImageFileModel> saveImageFile);
	}
}
