using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Quantum.Core.Models;
using Quantum.Core.Models.File;
using Quantum.Data.Entities;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
    public interface IFileService
	{
		Task<string> InsertFile(FileModel model, IIdentity identity);

		Task UpdateFile(string id, FileModel model, IIdentity identity);

		Task DeleteFile(string id, IIdentity identity);

		bool ValidateFile(IFormFile file);

		Task<File> GetFile(string id);
        void ValidateItemFile(IFormFile file);
    }
}
