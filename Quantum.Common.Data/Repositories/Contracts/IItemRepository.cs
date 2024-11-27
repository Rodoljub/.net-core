using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
using Quantum.Data.Repositories.Common.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IItemRepository : IBaseRepository<Item, IdentityUser>
    {
		Task<ItemViewModel> GetItemViewModel(string itemId, string userId);

		Task<int> GetItemViews(string itemId);

		Task<Item> InsertItem(Item item, IdentityUser user);

		Task<Item> UpdateItem(Item item, IdentityUser user);

		Task Delete(Item item, IdentityUser user);

		Task DeleteItemMaxReported(string itemId);

		Task<Item> GetItemById(string itemId);

		Task<Item> GetItemWithCommentIncludedByItemId(string itemId);

		Task UpdateItemAggregation(Item item);

		//Task<List<Item>> GetItemsForAnalysis(int take);

		Task<Item> GetItemByFileId(string fileId);

		Task Undelete(Item item, IdentityUser user);
    }
}
