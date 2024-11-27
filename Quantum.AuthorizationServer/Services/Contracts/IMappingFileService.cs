using Quantum.AuthorizationServer.Models;
using Quantum.Data.Entities;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services.Contracts
{
    public interface IMappingFileService
	{
		Task<UserModel> AddUserProfileserModelMappings(UserProfile userProfile, string imagePath, bool userEmailConfirmed);
	}
}
