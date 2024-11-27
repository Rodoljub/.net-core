using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
    public class FileTypeRepository : BaseRepository<FileType>, IFileTypeRepository
    {
        private QDbContext _context;

        public FileTypeRepository(QDbContext context)
            : base(context)
        {
            _context = context;
        }

		public async Task InsertFileType(FileType fileType, IdentityUser user)
		{
			await base.Insert(fileType, user);
		}

		public async Task UpdateFileType(FileType fileType, IdentityUser user)
		{
			await base.Update(fileType, user);
		}

		public async Task DeleteFileType(string fileTypeId, IdentityUser user)
		{
			var fileType = await GetFileTypeById(fileTypeId);

			await base.Delete(fileTypeId, user);
		}

		public async Task<FileType> GetFileTypeById(string fileTypeId)
		{
			var fileType = await base.GetById(fileTypeId);

			if (fileType == null)
			{
				//throw new 
			}

			return fileType;
		}


		public async Task<FileType> GetFileTypeByName(string name)
        {
            var fileType =  await base.Query(ft => ft.Name == name && !ft.IsDeleted)
                 .FirstOrDefaultAsync();

			if (fileType == null || fileType == default(FileType))
			{

			}

			return fileType;
        }
    }
}
