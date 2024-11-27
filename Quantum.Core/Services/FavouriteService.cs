using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Services.Contracts;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
	public class FavouriteService : IFavouriteService
	{
		private IMapper _mapper;
		private IFavouriteRepository _favouriteRepo;
		private IItemRepository _itemRepo;
		private IUserManagerService _userMgrServ;
		private ICLRTypeRepository _clrTypeRepo;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public IServiceProvider Services { get; }
		public IBackgroundTaskQueue Queue { get; }

		public FavouriteService(
			IMapper mapper,
			IFavouriteRepository favouriteRepo,
			IItemRepository itemRepo,
			IUserManagerService userMgrServ,
			ICLRTypeRepository clrTypeRepo,
			IServiceScopeFactory serviceScopeFactory,
			IServiceProvider services,
			IBackgroundTaskQueue queue
		)
		{
			_mapper = mapper;
			_favouriteRepo = favouriteRepo;
			_itemRepo = itemRepo;
			_userMgrServ = userMgrServ;
			_clrTypeRepo = clrTypeRepo;
			_serviceScopeFactory = serviceScopeFactory;
			Services = services;
			Queue = queue;
		}

		/// <summary>
		/// Map and Add or Remove Favourite
		/// </summary>
		/// <param name="model"></param>
		/// <param name="clrTypeId"></param>
		/// <param name="user"></param>
		/// <returns>Favourite</returns>
		public async Task AddOrRemoveFavourite(FavouriteModel model, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);

			var clrType = await _clrTypeRepo.GetClrTypeByName(model.Type);

			var favourite = await _favouriteRepo.GetByEntityId(model.EntityId, user.Id);

			if (favourite == null)
			{
				favourite = await MapFavourite(model, clrType.ID, user.Id);
				await _favouriteRepo.Insert(favourite, user);
			}
			else
			{
				if (favourite.IsDeleted)
				{
					var favouriteOrderNumberMax = await _favouriteRepo.GetFavouriteOrderNumberMax(user.Id);
					favourite.OrderNumber = favouriteOrderNumberMax + 1;
				}

				await _favouriteRepo.Update(favourite, user);
			}

			UpdateItemFavouriteAggregationinBackground(clrType.Name, favourite, user);
		}

		private void UpdateItemFavouriteAggregationinBackground(string clrTypeName, Favourite favourite, IdentityUser user)
		{
			if (clrTypeName == typeof(Item).Name)
			{
				Queue.QueueBackgroundWorkItem(async token =>
				{
					using (var scope = _serviceScopeFactory.CreateScope())
					{
						var scopedServices = scope.ServiceProvider;

						var scopedFavouriteRepo = scopedServices.GetRequiredService<IFavouriteRepository>();

						var scopedItemRepo = scopedServices.GetRequiredService<IItemRepository>();

						var scopedItemsListRepo = scopedServices.GetRequiredService<IItemsListRepository>();

						var scopedUserProfileRepo = scopedServices.GetRequiredService<IUserProfileRepository>();

						var favouritesCount = await scopedFavouriteRepo.CountFavouritesByEntityId(favourite.EntityId);

						var item = await scopedItemRepo.GetItemById(favourite.EntityId);

						item.FavouritesCount = favouritesCount;

						await scopedItemRepo.UpdateItemAggregation(item);

						var userProfileItemsFavouritesCount = await scopedItemsListRepo.CountUserItemsFavouritesByUserId(item.CreatedById);

						var userProfile = await scopedUserProfileRepo.GetByUserIdNoTrack(item.CreatedById);

						userProfile.FavouritesCount = userProfileItemsFavouritesCount;

						await scopedUserProfileRepo.Update(userProfile, user);
					}
				});
			}
		}

		private async Task<Favourite> MapFavourite(FavouriteModel model, string clrTypeId, string userId)
		{
			var favouriteOrderNumberMax = await _favouriteRepo.GetFavouriteOrderNumberMax(userId);

			var favouriteMapped = _mapper.Map<FavouriteModel, Favourite>(model, opt => FavouriteMapping(opt, clrTypeId, favouriteOrderNumberMax));

			return await Task.FromResult(favouriteMapped);
		}

		private void FavouriteMapping(IMappingOperationOptions<FavouriteModel, Favourite> opt, string clrTypeId, int favouriteOrderNumberMax)
		{
			opt.Items.Add("EntityTypeID", clrTypeId);
			opt.Items.Add("OrderNumber", favouriteOrderNumberMax + 1);
		}

	}
}
