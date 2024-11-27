using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Quantum.Core.Models.Folder;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using Quantum.Data.Repositories.Contracts;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
	public class FolderService : IFolderService
	{
		private IFolderRepository _folderRepo;
		private IUserManagerService _userMgrServ;
		private IMapper _mapper;

		public FolderService(
			IFolderRepository folderRepo,
			IUserManagerService userMgrServ,
			IMapper mapper
		)
		{
			_folderRepo = folderRepo;
			_userMgrServ = userMgrServ;
			_mapper = mapper;
		}

		public async Task CreateFolder(FolderModel model, IIdentity identity)
		{
			if (!string.IsNullOrWhiteSpace(model.Name))
			{
				//ModelState.AddModelError("ErrorFolderName", "Folder Name cannot be empty");
				//_logger.LogError($"Folder Name cannot be empty");
				//return BadRequest(ModelState);
			}

			var folder = new Folder()
			{
				Name = model.Name,
				ParentId = model.ParentId
			};

			var user = await _userMgrServ.GetAuthUser(identity);

			var folderId = await _folderRepo.Insert(folder, user);

			//if (!string.IsNullOrWhiteSpace(folder.ID))
			//{
			//	return Ok();
			//}

			//ModelState.AddModelError("ErrorSaveFolder", "Folder is not saved");
			//_logger.LogError($"Folder is not saved");
			//return BadRequest(ModelState);
		}

		public async Task UpdateFolder(string id, FolderModel model, IIdentity identity)
		{
			if (!string.IsNullOrWhiteSpace(model.Name))
			{
				//ModelState.AddModelError("ErrorFolderName", "Folder Name cannot be empty");
				//_logger.LogError($"Folder Name cannot be empty");
				//return BadRequest(ModelState);
			}

			var folder = new Folder();

			Folder existingFolder = null;

			existingFolder = await _folderRepo.GetById(id);

			if (existingFolder == null)
			{
				//_logger.LogError($"Folder was not found");
				//ModelState.AddModelError("FolderExistingError", "Folder was not found");
				//return BadRequest(ModelState);
			}

			folder = existingFolder;

			var user = await _userMgrServ.GetAuthUser(identity);

			if (existingFolder.CreatedById == user.Id)
				await _folderRepo.Update(_mapper.Map<Folder>(folder), user);

			//_logger.LogInformation($"Folder Updated successfully");

			//return Ok();
		}

		public async Task DeleteFolder(string id, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);
            var existingFolder = await _folderRepo.GetById(id);

            if (existingFolder.CreatedById == user.Id)
                await _folderRepo.Delete(id, user);
		}
	}
}
