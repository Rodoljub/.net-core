using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IFileRepository : IBaseRepository<Entities.File, IdentityUser>
    {
		Task<File> InsertFile(File file, IdentityUser user, bool save = true);

		Task UpdateFile(File file, IdentityUser user);

		Task DeleteFile(string fileId, IdentityUser user);

		Task<Entities.File> GetFileByName(string fileName);

		Task<FileDetails> GetFileDetailsByFileId(string fileId);

		Task<File> GetFileById(string fileId);

		Task<List<File>> GetFilesForAnalysis(int take);

        Task<List<File>> GetItemsFilesAnalyzingByUserId(string id);

		Task<File> GetProfileImageAnalyzingByUserId(string id);
	}
}
