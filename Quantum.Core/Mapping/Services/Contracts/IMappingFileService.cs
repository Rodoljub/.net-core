using Quantum.Core.Models;
using Quantum.Core.Models.File;
using Quantum.Data.Entities;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services.Contracts
{
	public interface IMappingFileService
	{
		Task<File> MapFileFromFileModel(FileModel model, byte[] fileContent, string fileName);

		Task<File> MapFileFromByteArray(byte[] userImageByteArray, string fileExstension, string fileTypeId, string fileName);

	}
}
