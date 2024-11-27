using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Quantum.Core.Models.Auth;
using Quantum.Data.Entities;
using System.Text.RegularExpressions;

namespace Quantum.Core.Mapping.Profiles
{
    public class UserMappingProfile : Profile
	{
		public UserMappingProfile()
		{
			CreateMap<RegisterModel, IdentityUser>()
				.ForMember(iu => iu.UserName,
				opt => opt.MapFrom((src, dest, destMember, resContext) => src.Email));

			CreateMap<RegisterModel, UserProfile>();

			CreateMap<IdentityUser, UserProfile>()
				.ForMember(up => up.UrlSegment,
				opt => opt.MapFrom((src, dest, destMember, resContext) =>
				$"{Regex.Replace(src.UserName.ToLower().Split(new char[] { '@' })[0], @"\s+", "")}"));

		}
	}
}
