using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Entities.Common;
using Quantum.Data.Repositories.Common.Contracts;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface ILikeRepository : IBaseRepository<Like, IdentityUser> 
    {
		Task Update(Like like, IdentityUser user);

		Task<Like> GetByEntityId(string entityId, string userId);

        Task Remove<T>(T Entity, IdentityUser user) where T : BaseEntity;

		Task<bool> IsUserLiked(string userId, string entityId);

		Task<int> GetLikesCountByEntityId(string entityId);
	}
}
