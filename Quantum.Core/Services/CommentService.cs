using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using Quantum.Utility.Infrastructure.Exceptions;
using Quantum.Utility.Services;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
    public class CommentService : ICommentService
	{
		private ICommentRepository _commentRepo;
		private IUserManagerService _userMgrServ;
		private IUserProfileRepository _userProfileRepo;
		private IConfiguration _config;
		private IMappingCommentService _mappingCommentServ;
		private IItemRepository _itemRepo;
		private ICLRTypeRepository _clrTypeRepo;
		private IImageService _imageServ;
		private IMapper _mapper;
        private IItemsService _itemServ;
        private readonly IServiceScopeFactory _serviceScopeFactory;

		public IServiceProvider Services { get; }
		public IBackgroundTaskQueue Queue { get; }


        public CommentService(
			ICommentRepository commentRepo,
			IUserManagerService userMgrServ,
			IUserProfileRepository userProfileRepo,
			IConfiguration config,
			IMappingCommentService mappingCommentServ,
			IItemRepository itemRepo,
			ICLRTypeRepository clrTypeRepo,
			IImageService imageServ,
			IMapper mapper,
			IServiceScopeFactory serviceScopeFactory,
			IServiceProvider services,
			IBackgroundTaskQueue queue,
            IItemsService itemServ
        )
		{
			_commentRepo = commentRepo;
			_userMgrServ = userMgrServ;
			_userProfileRepo = userProfileRepo;
			_config = config;
			_itemRepo = itemRepo;
			_clrTypeRepo = clrTypeRepo;
			_imageServ = imageServ;
			_mapper = mapper;
			_mappingCommentServ = mappingCommentServ;
			_serviceScopeFactory = serviceScopeFactory;
            _itemServ = itemServ;
            Services = services;
			Queue = queue;
        }

		public async Task<CommentViewModel> AddComment(CommentModel commentModel, IIdentity identity)
		{
			var clrType = await _clrTypeRepo.GetClrTypeByName(commentModel.typeName);
			var user = await _userMgrServ.GetAuthUser(identity);
			var userProfileId = await _userProfileRepo.GetUserProfileIdByUserId(user.Id);

			var commentAreEnabled = await _itemRepo.Query(i => i.ID == commentModel.ParentId && !i.IsDeleted && i.EnableComments).AnyAsync();

            if (commentAreEnabled)
			{
				var commentMapped = await _mappingCommentServ.MapCommentFromCommentModel(commentModel, clrType.ID, userProfileId);

				var comment = await _commentRepo.InsertComment(commentMapped, user);

				UpdateCommentsAggregateInBackground(clrType, comment, true, user);

				return await _mappingCommentServ.MapCommentViewModelFromComment(comment, user.Id);
			}

			return default;
		}

		public async Task<CommentViewModel> UpdateComment(CommentViewModel model, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);
			var comment = await _commentRepo.GetById(model.ID);

            var commentAreEnabled = await _itemRepo.Query(i => i.ID == comment.ParentId && !i.IsDeleted && i.EnableComments).AnyAsync();

            if (comment.CreatedById == user.Id && commentAreEnabled)
			{
				comment.Content = model.Content;

				await _commentRepo.Update(comment, user);

				var commentViewModel = _mappingCommentServ.MapCommentViewModelFromComment(comment, user.Id).GetAwaiter().GetResult();
				
				return commentViewModel;
			}

			throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
				null,
				Errors.GeneralError,
				null);
		}

		public async Task<bool> DeleteComment(string id, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);
			var userProfile = await _userProfileRepo.GetById(user.Id);
            var comment = await _commentRepo.GetById(id);
            var item = await _itemRepo.GetById(comment.ParentId);
			
			if (comment.CreatedById == user.Id || item.UserProfileId == userProfile.ID)
			{
				await _commentRepo.DeleteComment(id, user);

				var clrType = await _clrTypeRepo.GetById(comment.ParentTypeID);

				UpdateCommentsAggregateInBackground(clrType, comment, false, user);

				return true;
			}

			return false;
		}

		public async Task<IEnumerable<CommentViewModel>> GetViewCommentsModels(string parentId, string[] initialCommentsIds, string typeName, int skip, IIdentity identity)
		{
			var userId = string.Empty;
			var user = await _userMgrServ.GetAuthUser(identity);
			if (user != null)
				userId = user.Id;

			var clrType = await _clrTypeRepo.GetClrTypeByName(typeName);

			if (clrType.Name == typeof(Item).Name && !((await _itemRepo.GetById(parentId)).EnableComments))
			{
				return default;
			}
            
            int commentPageSize = _config.GetAsInteger("Application:CommentPageSize", 9);

			//var commentsCount = await _commentRepo.CountComments(parentId, clrType.ID);

			var comments = await _commentRepo.GetChildComments(skip, commentPageSize, initialCommentsIds, parentId, clrType.ID);
           
            var viewComents = comments.Select(co => _mappingCommentServ.MapCommentViewModelFromComment(co, userId).GetAwaiter().GetResult());

			return viewComents;
		}

		private void UpdateCommentsAggregateInBackground(CLR_Type clrType, Comment comment, bool isCommentAdd, IdentityUser user)
		{
			if (clrType.Name == typeof(Item).Name)
			{
				Queue.QueueBackgroundWorkItem(async token =>
				{
					using(var scope = _serviceScopeFactory.CreateScope())
					{
						var scopedServices = scope.ServiceProvider;

						var scopedItemRepo = scopedServices.GetRequiredService<IItemRepository>();

						var scopedItemsListRepo = scopedServices.GetRequiredService<IItemsListRepository>();

						var scopedUserProfileRepo = scopedServices.GetRequiredService<IUserProfileRepository>();

						var item = await scopedItemRepo.GetItemWithCommentIncludedByItemId(comment.ParentId);

						item.CommentsCount = item.Comments.Where(c => !c.IsDeleted && c.ParentId == item.ID && c.ParentTypeID == clrType.ID).Count();

						await scopedItemRepo.UpdateItemAggregation(item);

						var userProfileItemsCommentsCount = await scopedItemsListRepo.CountUserItemsCommentsByUserId(item.CreatedById);

						var userProfile = await scopedUserProfileRepo.GetByUserId(item.CreatedById);

						userProfile.CommentsCount = userProfileItemsCommentsCount;

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

						var scopedCommentRepo = scopedServices.GetRequiredService<ICommentRepository>();

						await scopedCommentRepo.UpdateParentCommentChildCount(comment.ParentId, isCommentAdd);
					}
				});
				//Task.Run(async () =>
				//{
				//	using (var scope = Services.CreateScope())
				//	{
				//		var scopedCommentRepo =
				//			scope.ServiceProvider
				//				.GetRequiredService<ICommentRepository>();

				//		await scopedCommentRepo.UpdateParentCommentChildCount(comment.ParentId, isCommentAdd);
				//	}
				//});
			}
		}
	}
}
