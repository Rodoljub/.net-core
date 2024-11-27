using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Models.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IItemsListRepository
    {
        Task<List<ItemViewModel>> GetLatestItems(int skip, int take, string userId, int itemsCount, int width = 320);

        Task<List<ItemViewModel>> GetUserItemsByUserId(int skip, int take, string userId, int itemsCount, int width = 320);

        Task<List<ItemViewModel>> GetItemsRelatedToItemTagsByItemId(int skip, int take, string itemId, string userId, int width = 320);

        Task<List<ItemViewModel>> GetSearchItems(int skip, int take, SearchRequestModel parsedQuery, List<ItemTagsCountModel> matchingItemTags, IEnumerable<string> idsOfTagsMatchingItems, string userId, int itemsCount, int width = 320);

        Task<List<ItemViewModel>> GetUserFavouritesItemsIdsByUserId(int skip, int take, string userId, int itemsCount, int width = 320);

        Task<int> CountAllItems();

        Task<int> CountUserItemsByUserId(string userId);

        Task<int> CountSearchItems(SearchRequestModel parsedQuery, List<ItemTagsCountModel> matchingItemTags, IEnumerable<string> idsOfTagsMatchingItems);

        Task<int> CountUserFavouriteItemsByUserId(string userId);

        Task<int> CountUserItemsFavouritesByUserId(string userId);

        Task<int> CountUserItemsLikesByUserId(string userId);

        Task<int> CountUserItemsCommentsByUserId(string userId);

    }
}
