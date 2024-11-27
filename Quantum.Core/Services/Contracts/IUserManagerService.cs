using Microsoft.AspNetCore.Identity;
using Quantum.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
	public interface IUserManagerService
	{
		Task<(IdentityResult IdentityResult, IdentityUser User)> CreateUser(RegisterModel model, bool external = false);

		Task CreateUserClaims(IdentityUser user, RegisterModel model);

		Task<IdentityUser> GetAuthUser(IIdentity userIdentity);
	}
}
