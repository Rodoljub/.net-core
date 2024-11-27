using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Quantum.Core.Models.Auth;
using Quantum.Core.Models.UserProfile;
using Quantum.Data.Entities;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
	public interface IUserProfileService
    {
		Task CreateUserProfile(RegisterModel model, IdentityUser user);

		Task<UpdatedProfileModel> UpdateProfile(UserProfileModel model, IIdentity identity);

        Task<bool> ValidUpdateQuota(string userId);

		Task<UserProfileViewModel> GetUserProfileViewModel(IIdentity identity);

		Task<UserProfileViewModel> GetUserProfileViewModelByUrlSegment(string urlSegment);

		Task<UserProfileViewModel> GetUserProfileViewModelByEmail(string email);

		Task<string> SetUserProfileCounters();

		Task<string> CreateRandomUrlSegment(string email);

        Task UpdateAnalysedProfileImage(File file, ImageAnalysis imageAnalysis);
    }
}
