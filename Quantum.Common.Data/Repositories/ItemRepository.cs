using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Models.ReadModels;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
	{
		private QDbContext _context;
		private IConfiguration _config;
		private IUtilityService _utilServ;

		public ItemRepository(
			QDbContext context,
			IConfiguration config,
			IUtilityService utilServ
		) : base(context)
		{
			_context = context;
			_config = config;
			_utilServ = utilServ;
		}

		public async Task<ItemViewModel> GetItemViewModel(string itemId, string userId)
		{

            var displayDateTimeFormat = _config["Application:DisplayDateTimeFormat"];

            var itemViewQuery = _context.Items.Where(i => !i.IsDeleted && i.ID == itemId && i.File != null && i.File.FileDetails != null)
				.AsNoTracking()
				.Select(i => new ItemViewModel()
				{
					CommentMostReply = i.Comments.Where(c => !c.IsDeleted)
						.OrderByDescending(c => c.ChildCount).ThenByDescending(c => c.CreatedDate)
						.Select(c => new CommentViewModel()
						{
							ChildCount = c.ChildCount,
							Content = c.Content,
                            //CreatedById = c.CreatedById,
                            //CreatedDate = c.CreatedDate.ToString(displayDateTimeFormat),
							DateFormat = displayDateTimeFormat,
                            CreatedDateD = c.CreatedDate,
                            ID = c.ID,
							LikeCount = c.LikeCount,
							UserLiked = c.Likes.Where(l => !l.IsDeleted && l.CreatedById == userId).Any(),
							UserProfile = new UserProfileEntityViewModel()
							{
								Name = c.UserProfile.Name,
								UrlSegment = c.UserProfile.UrlSegment,
								UserImagePath = c.UserProfile.ImageFileId,
								UserEntityOwner = c.CreatedById == userId,
							}
						}).FirstOrDefault() ?? new CommentViewModel(),
					CommentsCount = i.CommentsCount,
                    //CreatedDate = i.CreatedDate.ToString(displayDateTimeFormat),
                    DateFormat = displayDateTimeFormat,
                    CreatedDateD = i.CreatedDate,
                    UserFavourite = i.Favourites.Where(f => !f.IsDeleted && f.CreatedById == userId).Any(),
					FavouritesCount = i.FavouritesCount,
					ItemFilePath = i.FileID,
					FileDetails = new FileDetailsViewModel()
					{
						Color = i.File.FileDetails.Color,
						Height = i.File.FileDetails.Height,
						Width = i.File.FileDetails.Width,
						ImageAnalysisRaw = i.File.FileDetails.ImageAnalysis
					},
					Id = i.ID,
					Tags = i.ItemTags.Where(it => !it.IsDeleted && it.Display).Select(it => it.Tag.Name),
					LastModified = i.LastModified,
					UserLiked = i.Likes.Where(l => !l.IsDeleted && l.CreatedById == userId).Any(),
					LikesCount = i.LikesCount,
					Title = i.Title,
					Description = i.Description,
					TotalCount = 0,
					UserProfile = new UserProfileEntityViewModel()
					{
						Name = i.UserProfile.Name,
						UrlSegment = i.UserProfile.UrlSegment,
						UserImagePath = i.UserProfile.ImageFileId,
						UserEntityOwner = i.CreatedById == userId
					}
				});


			return await itemViewQuery.FirstOrDefaultAsync();
        }

		public override async Task<int> Delete(string id, IdentityUser user, bool save = true)
		{
			var entity = await GetById(id, false);

			if (entity != null && !entity.IsDeleted)
			{
				entity.IsDeleted = true;
				entity.DeletedBy = user;
				entity.DeletedOn = DateTime.UtcNow;
				_context.Entry(entity).State = EntityState.Modified;
			}

            if (save)
            {
                return await this.Save();
            }

            return await Task.FromResult(0);
        }


        public async Task<int> GetItemViews(string itemId)
        {
            return await Query(vi => !vi.IsDeleted && vi.ID == itemId)
                .CountAsync();
        }

		public async Task<Item> InsertItem(Item item, IdentityUser user)
		{
			await base.Insert(item, user);

			return item;
		}

		public async Task<Item> UpdateItem(Item item, IdentityUser user)
		{
			if (item.CreatedById == user.Id)
			{
				await base.Update(item, user);
			}
			else
			{
				ThrowItemNotUpdatedExcption(item, user);
			}

			return item;
		}

		public async Task Delete(Item item, IdentityUser user)
		{
			if (user.Id == item.CreatedById)
				await base.Delete(item.ID, user);
			else
				ThrowItemNotDeletedException(item.ID, user.Id);
		}

		public async Task DeleteItemMaxReported(string itemId)
		{
			await base.Delete(itemId, null);
		}

		public async Task<Item> GetItemById(string itemId)
		{
			var item = await base
							.Query(i => !i.IsDeleted && i.ID == itemId)
							.Include(i => i.File)
							.Include(i => i.ItemTags)
							.ThenInclude(it => it.Tag)
							.Include(i => i.Comments)
							.FirstOrDefaultAsync();

			if (item == null)
			{
				//	//throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
				//	//							   null,
				//	//							   Errors.ErrorItemNotFound,
				//	//							   null,
				//	//							   $"Item not found for itemId: {itemId}");
			}

			return item;
		}

		public async Task<Item> GetItemWithCommentIncludedByItemId(string itemId)
		{
			var item = await base.Query(i => !i.IsDeleted && i.ID == itemId)
					.Include(i => i.Comments)
					.FirstOrDefaultAsync();

			return item;
		}

		public async Task UpdateItemAggregation(Item item)
		{
			await base.Update(item, null);
		}




		private static void ThrowItemNotDeletedException(string itemId, string userId)
		{
			//throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
			//	   null,
			//	   Errors.ErrorItemWasNotDeleted,
			//	   null,
			//	   $"Item was not deleted for itemId: {itemId}, by userId: {userId}");
		}

		private static void ThrowItemNotUpdatedExcption(Item item, IdentityUser user)
		{
			//throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
			//		null,
			//		Errors.ErrorItemNotUpdated,
			//		null,
			//		$"Item was not updated for itemId: {item.ID}, by userId: {user.Id}");
		}

   //     public async Task<List<Item>> GetItemsForAnalysis(int take)
   //     {
			//return await Query(i => i.IsDeleted && !i.File.IsDeleted && i.File.FileDetailsId == null)
			//		.Include(i => i.File)
			//		.OrderBy(i => i.CreatedDate)
			//		.Take(take)
			//		.ToListAsync();
   //     }

		public async Task<Item> GetItemByFileId(string fileId)
        {
			return await Query(i => i.FileID == fileId)
						.Include(i => i.File)
						.FirstOrDefaultAsync();
        }

        public async Task Undelete(Item item, IdentityUser user)
        {
			item.IsDeleted = false;

			await Update(item, user);
        }

























        //private static void ThrowItemNotFoundException(string itemId)
        //{
        //	//throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
        //	//							   null,
        //	//							   Errors.ErrorItemNotFound,
        //	//							   null,
        //	//							   $"Item not found for itemId: {itemId}");
        //}







    }
}
