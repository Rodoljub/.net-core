using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
    public class FileRepository : BaseRepository<Entities.File>, IFileRepository
    {
        private QDbContext _context;
        private ILogger<FileRepository> _logger;

        public FileRepository(
            QDbContext context,
            ILogger<FileRepository> logger)
            : base(context)
        {
            _context = context;
            _logger = logger;
        }

		public async Task<File> InsertFile(File file, IdentityUser user, bool save = true)
		{
			await base.Insert(file, user, save);

			return file;
		}

		public async Task UpdateFile(File file, IdentityUser user)
		{
			await base.Update(file, user);
		}

		public async Task DeleteFile(string fileId, IdentityUser user)
		{
			await base.Delete(fileId, user);
		}

		public async Task<File> GetFileByName(string fileName)
        {
            return await base.Query(f => f.Name == fileName && !f.IsDeleted)
                .FirstOrDefaultAsync();
        }

		public async Task<FileDetails> GetFileDetailsByFileId(string fileId)
		{
			return await base.Query(f => !f.IsDeleted && f.ID == fileId)
				.Select(f => f.FileDetails)
				.FirstOrDefaultAsync();
		}

		public async Task<File> GetFileById(string fileId)
		{
			var file = await base.GetById(fileId);

			if (file == null)
			{
				throw new FileNotFoundException(
					HttpStatusCode.BadRequest, Errors.ErrorFileNotFound);
			}

			return file;
		}

		public async Task<List<File>> GetFilesForAnalysis(int take)
        {
			return await Query(f => !f.IsDeleted
                        && f.FileDetailsId == null
                        && (f.FileType.Name == FileTypes.Images.ProjectFile || f.FileType.Name == FileTypes.Images.ProfileImage))
					.Include(f => f.FileType)
					.OrderBy(i => i.CreatedDate)
					.Take(take)
					.ToListAsync();
		}

        public async Task<List<File>> GetItemsFilesAnalyzingByUserId(string id)
        {
			return await Query(f => !f.IsDeleted
									&& f.CreatedById == id
									&& f.FileDetailsId == null
                                    && (f.FileType.Name == FileTypes.Images.ProjectFile
									&& f.CreatedById == id
								))
								.Include(f => f.FileType)
								.OrderBy(i => i.CreatedDate)
								.ToListAsync();
		}

        public async Task<File> GetProfileImageAnalyzingByUserId(string id)
        {
			return await Query(f => !f.IsDeleted
									&& f.CreatedById == id
									&& f.FileDetailsId == null
                                    && (f.FileType.Name == FileTypes.Images.ProfileImage
									
								))
								.Include(f => f.FileType)
								.FirstOrDefaultAsync();

		}
    }
}
