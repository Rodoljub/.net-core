using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Quantum.Core.Models.Auth;
using Quantum.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
	public class UserManagerService : IUserManagerService
	{
		private UserManager<IdentityUser> _userMgr;
		private IMapper _mapper;
		public UserManagerService(
			UserManager<IdentityUser> userMgr,
			IMapper mapper
			)
		{
			_userMgr = userMgr;
			_mapper = mapper;
		}

		public async Task<(IdentityResult IdentityResult, IdentityUser User)> CreateUser(RegisterModel model, bool external = false)
		{

			IdentityUser user = _mapper.Map<RegisterModel, IdentityUser>(model);

			if (external)
            {
				user.EmailConfirmed = true;
            }

			IdentityResult result = await _userMgr.CreateAsync(user, model.Password);

			return (IdentityResult: result, User: user);
		}

		public async Task CreateUserClaims(IdentityUser user, RegisterModel model)
		{
			var claims = new List<Claim>()
				{
					new Claim(JwtRegisteredClaimNames.UniqueName, model.Name),
					new Claim(JwtRegisteredClaimNames.Email, user.Email)
				};

			await _userMgr.AddClaimsAsync(user, claims);
		}

		public async Task<IdentityUser> GetAuthUser(IIdentity userIdentity)
		{
			var identity = userIdentity as ClaimsIdentity;

			//var userEmailClaim = identity.Claims
			//	.FirstOrDefault(c => c.Type == ClaimTypes.Email);

			//if (userEmailClaim != null)
			//{
			//	var user = await _userMgr.FindByEmailAsync(userEmailClaim.Value);

			//	if (user != null)
			//	{
			//		return user;
			//	}
			//}

			var userSubClaim = identity.Claims
				.FirstOrDefault(c => c.Type == "sub");

			if (userSubClaim != null)
			{
				var user = await _userMgr.FindByIdAsync(userSubClaim.Value);

				if (user != null)
				{
					return user;
				}
			}

			return null;
		}
	}
}
