using AutoMapper;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Integration.Internal.Models;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services
{
	public class MappingImageService : IMappingImageService
	{
		private IMapper _mapper;
		public MappingImageService(
			IMapper mapper)
		{
			_mapper = mapper;
		}

		public async Task<SaveImageFileModel> MapSaveImageFileModelFromFile(Data.Entities.File imageFile, string base64Image, int width)
		{
			var saveImageFile = _mapper.Map<Data.Entities.File, SaveImageFileModel>(imageFile);
			saveImageFile.Base64Image = base64Image;
			saveImageFile.Type = imageFile.FileType.Name;
			saveImageFile.Width = width;

			return await Task.FromResult(saveImageFile);
		}
	}
}
