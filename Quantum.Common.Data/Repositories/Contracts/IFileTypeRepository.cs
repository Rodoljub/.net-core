using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IFileTypeRepository : IBaseRepository<FileType, IdentityUser>
    {
		Task InsertFileType(FileType fileType, IdentityUser user);

		Task UpdateFileType(FileType fileType, IdentityUser user);

		Task DeleteFileType(string fileTypeId, IdentityUser user);

		Task<FileType> GetFileTypeById(string fileTypeId);
		Task<FileType> GetFileTypeByName(string name);
    }
}
