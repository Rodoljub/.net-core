using AutoMapper;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models.Auth;
using Quantum.Core.Models.UserProfile;
using Quantum.Data.Entities;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services
{
	public class MappingUserProfileService : IMappingUserProfileService
	{
		private IMapper _mapper;

		public MappingUserProfileService(
			IMapper mapper)
		{
			_mapper = mapper;
		}

		public async Task<UserProfile> MapUserProfileFromUserProfile(UserProfile userProfile, string name, string profileImageFileId)
		{
			var mappedUserProfile = _mapper.Map<UserProfile, UserProfile>(userProfile);
			mappedUserProfile.Name = name;
			//mappedUserProfile.ImageFileId = profileImageFileId;
			return await Task.FromResult(mappedUserProfile);
		}

		public async Task<UserProfileViewModel> MapUserProfileViewModelFromUserProfile(UserProfile userProfile, string userImagePath)
		{
			var userProfileViewModel = _mapper.Map<UserProfile, UserProfileViewModel>(userProfile);
			userProfileViewModel.ImagePath = userImagePath;
			return await Task.FromResult(userProfileViewModel);
		}

	}
}
