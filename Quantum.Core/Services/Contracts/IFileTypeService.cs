using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
	public interface IFileTypeService
	{
		Task InsertFileType(string Name, IIdentity identity);

		Task UpdateFileType(string Name, string id, IIdentity identity);

		Task DeleteFileType(string id, IIdentity identity);
	}
}
