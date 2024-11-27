using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quantum.AuthorizationServer.Models;
using Quantum.AuthorizationServer.Services.Contracts;
using Quantum.Data.Entities;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services
{
    public class MappingFileService : IMappingFileService
	{
		private ILogger<MappingFileService> _logger;
		private IConfigurationRoot _config;
		private IMapper _mapper;

		public MappingFileService(
			ILogger<MappingFileService> logger,
			IConfigurationRoot config,
			IMapper mapper
		)
		{
			_logger = logger;
			_config = config;
			_mapper = mapper;
		}

		public async Task<UserModel> AddUserProfileserModelMappings(UserProfile userProfile, string imagePath, bool userEmailConfirmed)
		{
			var userModel = _mapper.Map<UserProfile, UserModel>(userProfile, opt => AddUserProfileserModelMappings(imagePath, userEmailConfirmed, opt));

			return await Task.FromResult(userModel);		
		}

		private void AddUserProfileserModelMappings(string imagePath, bool userEmailConfirmed, IMappingOperationOptions<UserProfile, UserModel> opt)
		{
			opt.Items.Add("ImagePath", imagePath);
			opt.Items.Add("UserEmailConfirmed", userEmailConfirmed);
		}

		//public async Task<SaveImageFileModel> AddSaveImageFileMappings(File userImageFile, string base64Image)
		//{
		//	var saveImageFile = _mapper.Map<File, SaveImageFileModel>(userImageFile, opt => AddSaveImageFileMappings(userImageFile, base64Image, opt));

		//	return await Task.FromResult(saveImageFile);
		//}

		//private void AddSaveImageFileMappings(File userImageFile, string base64Image, IMappingOperationOptions<File, SaveImageFileModel> opt)
		//{
		//	int width = _config.GetAsInteger($"Application:ImageSize:320", 320);
		//	opt.Items.Add("Base64Image", base64Image);
		//	opt.Items.Add("Type", userImageFile.FileType.Name);
		//	opt.Items.Add("Width", width);
		//}
	}
}
