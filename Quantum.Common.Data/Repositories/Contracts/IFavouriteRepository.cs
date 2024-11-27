using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IFavouriteRepository : IBaseRepository<Favourite, IdentityUser>
	{
        Task Update(Favourite favorite, IdentityUser user);

		Task<bool> IsUserFavourite(string userId, string entityId);

		Task<Favourite> GetByEntityId(string entityId, string userId);

		Task<int> GetFavouriteOrderNumberMax(string userId);

		Task<int> CountFavouritesByEntityId(string entityId);



	}
}
