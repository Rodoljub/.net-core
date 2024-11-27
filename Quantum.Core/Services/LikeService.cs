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
	public class LikeService : ILikeService
	{
		private ILikeRepository _likeRepo;
		private IUserManagerService _userMgrServ;
		private IItemRepository _itemRepo;
		private ICLRTypeRepository _clrTypeRepo;
		private IMapper _mapper;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public IServiceProvider Services { get; }
		public IBackgroundTaskQueue Queue { get; }


		public LikeService(
			ILikeRepository likeRepo,
			IUserManagerService userMgrServ,
			IMapper mapper,
			ICLRTypeRepository clrTypeRepo,
			IItemRepository itemRepo,
			IServiceScopeFactory serviceScopeFactory,
			IServiceProvider services,
			IBackgroundTaskQueue queue
		)
		{
			_likeRepo = likeRepo;
			_userMgrServ = userMgrServ;
			_mapper = mapper;
			_itemRepo = itemRepo;
			_clrTypeRepo = clrTypeRepo;
			_serviceScopeFactory = serviceScopeFactory;
			Services = services;
			Queue = queue;
		}

		public async Task AddOrRemoveLike(LikeModel model, IIdentity identity)
		{
			var clrType = await _clrTypeRepo.GetClrTypeByName(model.Type);

			var user = await _userMgrServ.GetAuthUser(identity);

			var like = await _likeRepo.GetByEntityId(model.EntityId, user.Id);

			if(like == null)
			{
				var likeMapped = _mapper.Map<LikeModel, Like>(model);
				likeMapped.EntityTypeID = clrType.ID;
				if (model.Type == typeof(Item).Name)
                {
					likeMapped.ItemID = model.EntityId;
                }

				if (model.Type == typeof(Comment).Name)
                {
					likeMapped.CommentID = model.EntityId;
                }

				await _likeRepo.Insert(likeMapped, user);
			}
			else
			{
				await _likeRepo.Update(like, user);
			}

			UpdateItemLikeAggregationInBackground(clrType, like, user);
		}

		private void UpdateItemLikeAggregationInBackground(CLR_Type clrType, Like like, IdentityUser user)
		{
			if (clrType.Name == typeof(Item).Name)
			{
				Queue.QueueBackgroundWorkItem(async token =>
				{
					using (var scope = _serviceScopeFactory.CreateScope())
					{
						var scopedServices = scope.ServiceProvider;

						var scopedLikeRepo = scopedServices.GetRequiredService<ILikeRepository>();

						var scopedItemRepo = scopedServices.GetRequiredService<IItemRepository>();

						var scopedItemsListRepo = scopedServices.GetRequiredService<IItemsListRepository>();


						var scopedUserProfileRepo = scopedServices.GetRequiredService<IUserProfileRepository>();

						var likesCount = await scopedLikeRepo.GetLikesCountByEntityId(like.EntityId);

						var item = await scopedItemRepo.GetItemById(like.EntityId);

						item.LikesCount = likesCount;

						await scopedItemRepo.UpdateItemAggregation(item);

						var userProfileItemsLikesCount = await scopedItemsListRepo.CountUserItemsLikesByUserId(item.CreatedById);

						var userProfile = await scopedUserProfileRepo.GetByUserId(item.CreatedById);

						userProfile.LikesCount = userProfileItemsLikesCount;

						await scopedUserProfileRepo.Update(userProfile, user);
					}
				});
			}

			if (clrType.Name == typeof(Comment).Name)
			{
				Queue.QueueBackgroundWorkItem(async token =>
				{
					using (var scope = _serviceScopeFactory.CreateScope())
					{
						var scopedServices = scope.ServiceProvider;

						var scopedLikeRepo = scopedServices.GetRequiredService<ILikeRepository>();

						var scopedCommentRepo = scopedServices.GetRequiredService<ICommentRepository>();

						var likesCount = await scopedLikeRepo.GetLikesCountByEntityId(like.EntityId);

						var comment = await scopedCommentRepo.GetById(like.EntityId);

						comment.LikeCount = likesCount;

						await scopedCommentRepo.Update(comment, null);
					}
				});
				
			}
		}
	}
}
