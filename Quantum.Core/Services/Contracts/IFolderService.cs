using Microsoft.AspNetCore.Identity;
using Quantum.Core.Models.Folder;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
	public interface IFolderService
	{
		Task CreateFolder(FolderModel model, IIdentity identity);

		Task UpdateFolder(string id, FolderModel model, IIdentity identity);

		Task DeleteFolder(string id, IIdentity identity);
	}
}
