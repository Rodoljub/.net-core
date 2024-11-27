using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
    public class FavouriteRepository : BaseRepository<Favourite>, IFavouriteRepository
	{
		private QDbContext _context;

		public FavouriteRepository(QDbContext context)
			: base(context)
		{
			_context = context;
		}

        public async Task Update(Favourite favourite, IdentityUser user)
        {
			if (favourite.IsDeleted)
			{
				favourite.IsDeleted = false;
			}
			else
			{
				favourite.IsDeleted = true;
			}

			await base.Update(favourite, user);
		}

		public async Task<bool> IsUserFavourite(string userId, string entityId)
		{
			return await base.Query(uf => !uf.IsDeleted && uf.CreatedById == userId && uf.EntityId == entityId)
				.AnyAsync();
		}

		public async Task<Favourite> GetByEntityId(string entityId, string userId)
		{
			return await base.Query(f => f.EntityId == entityId && f.CreatedById == userId)
				.FirstOrDefaultAsync();
		}

		public async Task<int> GetFavouriteOrderNumberMax(string userId)
		{
			return await base.Query(f => !f.IsDeleted && f.CreatedById == userId)
				.OrderByDescending(f => f.OrderNumber)
				.Select(f => f.OrderNumber)
				.FirstOrDefaultAsync();
		}

		public async Task<int> CountFavouritesByEntityId(string entityId)
		{
			return await base.Query(f => !f.IsDeleted && f.EntityId == entityId)
				.CountAsync();
		}

	}
}
