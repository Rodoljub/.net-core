using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IUserProfileRepository : IBaseRepository<UserProfile, IdentityUser>
	{
		Task<UserProfile> GetByUserId(string id);

        Task<UserProfile> GetByUserIdNoTrack(string id);

        Task<UserProfile> GetByUserIdAsNoTracking(string id);

        Task<bool> UrlSegmentAlreadyExists(string name);

        Task<string> GetUserProfileIdByUserId(string userId);

        Task<UserProfile> GetUserProfileByEmail(string email);

        Task<UserProfile> GetUserProfileByUrlSegment(string urlSegment);

    }
}
