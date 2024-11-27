using Microsoft.AspNetCore.Identity;
using Quantum.AuthorizationServer.Models;
using Quantum.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services.Contracts
{
    public interface IUserProfileService
    {
        Task<bool> UrlSegmentExists(string segmentName);

        Task<(IdentityResult IdentityResult, IdentityUser User)> CreateUser(RegisterModel model);

        Task CreateUserProfile(RegisterModel model, IdentityUser user);

        Task CreateUserClaims(IdentityUser user, RegisterModel model);

        Task<UserModel> GetUserProfileByEmail(string email, bool getProfileImage = false);
    }
}
