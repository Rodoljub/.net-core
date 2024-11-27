using AutoMapper;
using Quantum.Core.Models.UserProfile;
using Quantum.Data.Entities;

namespace Quantum.Core.Mapping.Profiles
{
	public class UserProfileMappingProfile : Profile
	{
		public UserProfileMappingProfile()
		{
			CreateMap<UserProfile, UserProfile>();

			CreateMap<UserProfile, UserProfileViewModel>();
		}
	}
}
