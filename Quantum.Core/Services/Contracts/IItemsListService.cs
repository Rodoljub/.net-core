using Quantum.Core.Models;
using Quantum.Data.Models.ReadModels;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
    public interface IItemsListService
	{
		Task<List<ItemViewModel>> GetLatestItems(int skip, IIdentity identity);

		Task<List<ItemViewModel>> GetPortfolioItems(int skip, IIdentity identity);

		Task<List<ItemViewModel>> GetPortfolioAnonymousItems(int skip, IIdentity identity, string userName);

		Task<List<ItemViewModel>> GetRelatedItems(string itemId, int skip, IIdentity identity);

		Task<List<ItemViewModel>> GetFavouritesItems(int skip, IIdentity identity);

		Task<List<ItemViewModel>> GetSearchItems(int skip, IIdentity identity, string searchQuery);
	}
}
