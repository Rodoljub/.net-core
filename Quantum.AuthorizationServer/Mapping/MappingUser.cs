using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Quantum.AuthorizationServer.Models;
using Quantum.Data.Entities;
using Quantum.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Mapping
{
	public class MappingUser : Profile
	{
		public MappingUser()
		{
			CreateMap<RegisterModel, IdentityUser>()
				.ForMember(iu => iu.UserName, opt => opt.MapFrom((src, dest, destMember, resContext) => src.Email));

			CreateMap<RegisterModel, UserProfile>();

			CreateMap<IdentityUser, UserProfile>()
				.ForMember(up => up.UrlSegment, opt => opt.MapFrom((src, dest, destMember, resContext) =>
				$"{Regex.Replace(src.UserName.ToLower().Split(new char[] { '@' })[0], @"\s+", "")}"));

			CreateMap<UserProfile, UserModel>()
				.ForMember(usm => usm.UserImage, opt => opt.MapFrom((src, dest, destMember, resContext) => resContext.Items["ImagePath"]))
				.ForMember(usm => usm.EmailConfirmed, opt => opt.MapFrom((src, dest, destMember, resContext) => resContext.Items["UserEmailConfirmed"]));
		}
	}
}
