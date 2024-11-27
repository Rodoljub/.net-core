using Quantum.Integration.Internal.Models;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services.Contracts
{
	public interface IMappingImageService
	{
		Task<SaveImageFileModel> MapSaveImageFileModelFromFile(Data.Entities.File imageFile, string base64Image, int width);
	}
}
