using AutoMapper;
using Microsoft.AspNetCore.Http;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Core.Models.File;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services
{
	public class MappingFileService : IMappingFileService
	{
		private IMapper _mapper;
		private IFileTypeRepository _fileTypeRepo;

		public MappingFileService(
			IMapper mapper,
			IFileTypeRepository fileTypeRepo
		)
		{
			_mapper = mapper;
			_fileTypeRepo = fileTypeRepo;
		}

		public async Task<Data.Entities.File> MapFileFromFileModel(FileModel model, byte[] fileContent, string fileName)
		{
			//var folder = await _folderRepo.GetByIDAsync(model.FolderId);

			var fileType = await _fileTypeRepo.GetFileTypeById(model.FileTypeId);

			var file = _mapper.Map<FileModel, Data.Entities.File>(model);
			
			file.file = fileContent;
			//file.Folder = folder;
			file.Name = fileName;
			file.TypeId = fileType.ID;
			return await Task.FromResult(file);
		}

		public async Task<Data.Entities.File> MapFileFromByteArray(byte[] file, string fileExstension, string fileTypeId, string fileName)
		{
			var fileEntity = _mapper.Map<byte[], Data.Entities.File>(file);
			fileEntity.Name = fileName;
			fileEntity.Extension = fileExstension;
			fileEntity.TypeId = fileTypeId;

			return await Task.FromResult(fileEntity);
		}
	}
}
