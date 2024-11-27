using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Core.Models.File;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using Quantum.Utility.Infrastructure.Exceptions;
using Quantum.Utility.Services.Contracts;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using File = Quantum.Data.Entities.File;

namespace Quantum.Core.Services
{
    public class FileService : IFileService
	{
		private IConfiguration _config;
		private IMapper _mapper;
		private IUtilityService _utilServ;
		private IUserManagerService _userMgrServ;
		private IFileDetailsRepository _fileDetailsRepo;
		private IFileTypeRepository _fileTypeRepo;
		private IFileRepository _fileRepo;
		private IMappingFileService _mappingFileServ;

		public FileService(
			IConfiguration config,
			IMapper mapper,
			IUserManagerService userMgrServ,
			IUtilityService utilServ,
			IFileDetailsRepository fileDetailsRepo,
			IFileTypeRepository fileTypeRepo,
			IFileRepository fileRepo,
			IMappingFileService mappingFileServ
		)
		{
			_config = config;
			_mapper = mapper;
			_userMgrServ = userMgrServ;
			_utilServ = utilServ;
			_fileDetailsRepo = fileDetailsRepo;
			_fileTypeRepo = fileTypeRepo;
			_fileRepo = fileRepo;
			_mappingFileServ = mappingFileServ;
		}

		public async Task<string> InsertFile(FileModel model, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);

			ValidateFile(model.File);

			var formFile = model.File;

			var filebyteArray = _utilServ.StreamToByteArray(formFile.OpenReadStream(), (int)formFile.Length);

			var mappedFile = await _mappingFileServ.MapFileFromFileModel(model, filebyteArray, formFile.FileName);

			var file = await _fileRepo.InsertFile(mappedFile, user);

			return file.ID;
		}

		public async Task UpdateFile(string id, FileModel model, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);

			var file = await _fileRepo.GetFileById(id);

			if (file.CreatedById == user.Id)
			{
				ValidateFile(model.File);

				var formFile = model.File;

				var filebyteArray = _utilServ.StreamToByteArray(formFile.OpenReadStream(), (int)formFile.Length);

				file.file = filebyteArray;

				await _fileRepo.Update(file, user);
			}
		}

		public async Task DeleteFile(string id, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);
			var file = await _fileRepo.GetFileById(id);

			if (file.CreatedById == user.Id)
				await _fileRepo.Delete(id, user);
		}

		public bool ValidateFile(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
				   null,
				   Errors.ErrorFileIsEmpty,
				   null,
				   $"File is Empty");
			}

			return true;
		}

		public async Task<File> GetFile(string id)
        {
			return await _fileRepo.Query(f => !f.IsDeleted && f.ID == id)
					.Include(f => f.Item)
					.FirstOrDefaultAsync();
        }

        public void ValidateItemFile(IFormFile file)
        {
			ValidateFile(file);
			ValidateFileMaxLength(file);
        }

        private void ValidateFileMaxLength(IFormFile file)
        {
			var maxLength = _config.GetAsInteger("Application:AzureBlob:MaxFileSize");
			var maxLengthMb = _config["Application:AzureBlob:MaxFileSizeMb"];
			if (file.Length > maxLength)
			{
				throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
				   null,
				   Errors.ErrorFileIsTooLarge,
				   null,
				   $"File is too large. Max is {maxLengthMb}");
			}
		}
    }
}
