using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services.Contracts
{
    public interface IAuthService
    {
        bool CheckRefrashToken(System.Security.Principal.IIdentity userIdentity);

        Task<(bool Authenticated, string Error, IdentityUser User)> AuthenticateUser(string username, string password);

        Task ValidateLoginAttempt(IdentityUser User);

        Task<JwtSecurityToken> CreateUserToken(IdentityUser user, Boolean keepMeLoggedIn);
    }
}
