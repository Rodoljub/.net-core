
using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IViewRepository : IBaseRepository<View, IdentityUser>
	{
        //Task AddItemView(string itemId, IdentityUser user, string ipAddress = null);

        //Task<int> GetItemViews(string itemId);
    }
}
