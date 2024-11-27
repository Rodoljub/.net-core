using AutoMapper;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
	public class FileTypeService : IFileTypeService
	{

		private IMapper _mapper;
		private IUserManagerService _userMgrServ;
		private IFileTypeRepository _fileTypeRepo;

		public FileTypeService(
			IMapper mapper,
			IUserManagerService userMgrServ,
			IFileTypeRepository fileTypeRepo
		)
		{
			_mapper = mapper;
			_userMgrServ = userMgrServ;
			_fileTypeRepo = fileTypeRepo;
		}

		public async Task InsertFileType(string Name, IIdentity identity)
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				ThrowFileTypeNameEmptyException();
			}

			var user = await _userMgrServ.GetAuthUser(identity);

			FileType fileType = new FileType()
			{
				Name = Name
			};

			await _fileTypeRepo.InsertFileType(fileType, user);		
		}

		public async Task UpdateFileType(string Name, string fyleTypeId, IIdentity identity)
		{
			if (!string.IsNullOrWhiteSpace(Name))
			{
				ThrowFileTypeNameEmptyException();
			}

			var user = await _userMgrServ.GetAuthUser(identity);

			var fileType  = await _fileTypeRepo.GetFileTypeById(fyleTypeId);

			fileType.Name = Name;

			await _fileTypeRepo.UpdateFileType(fileType, user);
		}

		public async Task DeleteFileType(string id, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);

			await _fileTypeRepo.DeleteFileType(id, user);
		}

		private static void ThrowFileTypeNameEmptyException()
		{
			throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
				null,
				Errors.ErrorCreateFileTypeNameEmpty,
				null,
				"Error Create File Type, Name Empty");
		}
	}
}
