using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Data;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Models.ReadModels;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Extensions;
using Quantum.Utility.Services;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
    public class ItemsListService : IItemsListService
	{
		private QDbContext _context;
		private ILogger<ItemsListService> _logger;
        private IItemRepository _itemRepo;
		private IItemsListRepository _itemsListRepo;
		private IFavouriteRepository _favouriteRepo;
		private IMappingItemsService _mappingItemsServ;
		private IUserManagerService _userMgrServ;
		private IUserProfileService _userProfileServ;
		private IUserProfileRepository _userProfileRepo;
		private IConfiguration _config;
		private IImageService _imageServ;
		private IItemTagRepository _itemTagRepo;
		private ITagRepository _tagRepo;
        private ISearchService _searchServ;
		private IUtilityService _utilityServ;

		public ItemsListService(
			QDbContext context,
			ILogger<ItemsListService> logger,
            IItemRepository itemRepo,
			IItemsListRepository itemsListRepo,
			IFavouriteRepository favouriteRepo,
			IMappingItemsService mappingItemsServ,
			IUserManagerService userMgrServ,
			IUserProfileService userProfileServ,
			IUserProfileRepository userProfileRepo,
			IConfiguration config,
			IImageService imageServ,
			IItemTagRepository itemTagRepo,
			ITagRepository tagRepo,
            ISearchService searchServ,
			IUtilityService utilityServ
        )
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
			_itemsListRepo = itemsListRepo ?? throw new ArgumentNullException(nameof(itemsListRepo));
			_favouriteRepo = favouriteRepo ?? throw new ArgumentNullException(nameof(favouriteRepo));
			_mappingItemsServ = mappingItemsServ ?? throw new ArgumentNullException(nameof(mappingItemsServ));
			_userProfileServ = userProfileServ ?? throw new ArgumentNullException(nameof(userProfileServ));
			_userProfileRepo = userProfileRepo ?? throw new ArgumentNullException(nameof(userProfileRepo));
			_userMgrServ = userMgrServ ?? throw new ArgumentNullException(nameof(userMgrServ));
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_imageServ = imageServ ?? throw new ArgumentNullException(nameof(imageServ));
			_itemTagRepo = itemTagRepo ?? throw new ArgumentNullException(nameof(itemTagRepo));
			_tagRepo = tagRepo ?? throw new ArgumentNullException(nameof(tagRepo));
            _searchServ = searchServ ?? throw new ArgumentNullException(nameof(searchServ));
			_utilityServ = utilityServ ?? throw new ArgumentNullException(nameof(utilityServ));
		}

		public async Task<List<ItemViewModel>> GetLatestItems(int skip, IIdentity identity)
		{
			var take = _config.GetAsInteger("Application:LatestItemPageSize", 9);

			if (skip == 0 || skip >= take)
            {
                string userId = await GetAuthUserId(identity);

                var itemsCount = skip == 0 ? await _itemsListRepo.CountAllItems() : -1;//Task.FromResult<int>(-1);
				
				
				//Stopwatch myStopWatch = new Stopwatch();
				//myStopWatch.Reset();
				//myStopWatch.Start();

				//var items2 = await _itemRepo.Query(i => !i.IsDeleted)
				//.Include(i => i.ItemTags)
				//.Include(i => i.Comments)
				//.OrderByDescending(i => i.LastModified)
				//.Skip(skip).Take(take)
				//.ToListAsync();

				//ItemViewModel[] viewItems2 = await MapViewItems(userId, itemsCount, items2.ToList(), tags);

				//myStopWatch.Stop();
				//Console.WriteLine("old Way repo: " + myStopWatch.Elapsed.Milliseconds);


				var viewItems = await _itemsListRepo.GetLatestItems(skip, take, userId, itemsCount);

				

				//ItemViewModel[] viewItems = items.ToArray();
					//await MapViewItems(userId, itemsCount, items.ToList(), tags);

                return viewItems;
            }

            return null;
		}

        private async Task<string> GetAuthUserId(IIdentity identity)
        {
            var userId = string.Empty;
            var user = await _utilityServ.GetAuthUser(identity);

            if (user != null)
            {
                userId = user.Id;
            }

            return userId;
        }

		public async Task<List<ItemViewModel>> GetPortfolioItems(int skip, IIdentity identity)
		{
			var take = _config.GetAsInteger("Application:PortfolioItemPageSize", 9);

			if (skip == 0 || skip >= take)
			{
				string userId = await GetAuthUserId(identity);

				var itemsCount = skip == 0 ? await _itemsListRepo.CountUserItemsByUserId(userId) : -1;

				var viewItems = await _itemsListRepo.GetUserItemsByUserId(skip, take, userId, itemsCount);

				return viewItems;
			}

			return null;
		}

		public async Task<List<ItemViewModel>> GetPortfolioAnonymousItems(int skip, IIdentity identity, string userName)
		{
			var take = _config.GetAsInteger("Application:PortfolioItemPageSize", 9);

			if (skip == 0 || skip >= take)
			{
				string userId = await GetAuthUserId(identity);

				var userProfile = await _userProfileRepo.GetUserProfileByUrlSegment(userName);

				var itemsCount = skip == 0 ? await _itemsListRepo.CountUserItemsByUserId(userProfile.CreatedById) : -1;

				var viewItems = await _itemsListRepo.GetUserItemsByUserId(skip, take, userProfile.CreatedById, itemsCount);

				return viewItems;
			}

			return null;
		}

		public async Task<List<ItemViewModel>> GetRelatedItems(string itemId, int skip, IIdentity identity)
		{
			var take = _config.GetAsInteger("Application:RelatedItemsPageSize", 9);
			var maxRelatedItems = _config.GetAsInteger("Application:MaxRelatedItemsPages", 2);

			if (skip < maxRelatedItems * take && skip <= take)
			{
				string userId = await GetAuthUserId(identity);

				var viewItems = await _itemsListRepo.GetItemsRelatedToItemTagsByItemId(skip, take, itemId, userId);

				return viewItems;
			}

			return null;
		}

		public async Task<List<ItemViewModel>> GetFavouritesItems(int skip, IIdentity identity)
		{
			var take = _config.GetAsInteger("Application:PortfolioItemPageSize", 9);

			if (skip == 0 || skip >= take)
			{
				string userId = await GetAuthUserId(identity);

				var itemsCount = skip == 0 ? await _itemsListRepo.CountUserFavouriteItemsByUserId(userId) : -1;

				var viewItems = await _itemsListRepo.GetUserFavouritesItemsIdsByUserId(skip, take, userId, itemsCount);

				return viewItems;
			}

			return null;
		}

		public async Task<List<ItemViewModel>> GetSearchItems(int skip, IIdentity identity, string searchQuery)
		{
			if (!string.IsNullOrWhiteSpace(searchQuery))
			{
				var take = _config.GetAsInteger("Application:LatestItemPageSize", 9);

				if (skip == 0 || skip >= take)
				{

					string userId = await GetAuthUserId(identity);

					var parsedQuery = _searchServ.ParseSearchRequest(searchQuery);

					var matchingItemTags = new List<ItemTagsCountModel>();

					if (parsedQuery.Tags.Any())
					{
						matchingItemTags = await GetMathedItemTagsByParsedQuery(parsedQuery);
					}

					if (matchingItemTags.Count() > 0)
					{
						parsedQuery.QueryCollection = new string[] { };
					}

					var idsOfTagsMatchingItems = matchingItemTags.Select(mit => mit.ItemId);

					var itemsCount = skip == 0 ? await _itemsListRepo.CountSearchItems(parsedQuery, matchingItemTags, idsOfTagsMatchingItems) : -1;

					var viewItems = await _itemsListRepo.GetSearchItems(skip, take, parsedQuery, matchingItemTags, idsOfTagsMatchingItems, userId, itemsCount);

					return viewItems;
				}

				return null;
			}
			return null;
		}

		public async Task<List<ItemTagsCountModel>> GetMathedItemTagsByParsedQuery(SearchRequestModel parsedQuery)
		{
			return await _context.Tags.Where(t => !t.IsDeleted && parsedQuery.Tags.Contains(t.Name.ToLower()))
									.AsNoTracking()
									.Include(t => t.ItemTags)
									.SelectMany(t => t.ItemTags)
									.GroupBy(it => it.ItemId)
									.Select(group => new ItemTagsCountModel
									{
										ItemId = group.Max(t => t.ItemId),
										MatchingTagsCount = group.Count()
									})
									.OrderByDescending(itc => itc.MatchingTagsCount)
									.ToListAsync();

		}
	}
}
