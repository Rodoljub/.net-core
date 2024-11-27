using Quantum.Core.Models.Auth;
using Quantum.Core.Models.UserProfile;
using Quantum.Data.Entities;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services.Contracts
{
	public interface IMappingUserProfileService
	{
		Task<UserProfile> MapUserProfileFromUserProfile(UserProfile userProfile, string name, string profileImageFileId);

		Task<UserProfileViewModel> MapUserProfileViewModelFromUserProfile(UserProfile userProfile, string userImagePath);

	}
}
