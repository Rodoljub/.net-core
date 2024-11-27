using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities;
using Quantum.Data.Entities.Common;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
    public class LikeRepository : BaseRepository<Like>, ILikeRepository
	{
		private QDbContext _context;

		public LikeRepository(QDbContext context) 
			: base(context)
		{
			_context = context;
		}

		public async Task Update(Like like, IdentityUser user)
		{
			if (like.IsDeleted)
			{
				like.IsDeleted = false;
			}
			else
			{
				like.IsDeleted = true;
			}

			await base.Update(like, user);
		}

		public async Task Remove<T>(T entity, IdentityUser user) where T : BaseEntity
		{
			var entityType = entity.GetType().Name;

			var like = await Query(l => l.EntityId == entity.ID && l.EntityType.Name == entityType && l.CreatedById == user.Id && l.IsDeleted == false)
                .FirstOrDefaultAsync();

			if(like != null)
			{
				await Delete(like.ID, user);
            }
        }

		public async Task<bool> IsUserLiked(string userId, string entityId)
		{
			return await base.Query(ul => !ul.IsDeleted && ul.CreatedById == userId && ul.EntityId == entityId)
					.AnyAsync();

		}

		public async Task<Like> GetByEntityId(string entityId, string userId)
		{
			return await base.Query(l => l.EntityId == entityId && l.CreatedById == userId)
				.FirstOrDefaultAsync();
		}

		public async Task<int> GetLikesCountByEntityId(string entityId)
		{
			return await base.Query(l => !l.IsDeleted && l.EntityId == entityId)
				.CountAsync();
		}
	}
}
