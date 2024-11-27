using Microsoft.CodeAnalysis.CSharp.Syntax;
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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
    public class ItemsListRepository : BaseRepository<Item>, IItemsListRepository
    {
        private QDbContext _context;
        private IConfiguration _config;
        private IUtilityService _utilServ;
        public ItemsListRepository(
            QDbContext context,
            IConfiguration config,
            IUtilityService utilServ
        ) : base(context)
        {
            _context = context;
            _config = config;
            _utilServ = utilServ;
        }

        public async Task<List<ItemViewModel>> GetLatestItems(int skip, int take, string userId, int itemsCount, int width = 320)
        {
            var displayDateTimeFormat = _config["Application:DisplayDateTimeFormat"];

            var itemsQuery = _context.Items.Where(i => !i.IsDeleted && i.File != null && i.File.FileDetails != null)
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
                    TotalCount = itemsCount,
                    UserProfile = new UserProfileEntityViewModel()
                    {
                        Name = i.UserProfile.Name,
                        UrlSegment = i.UserProfile.UrlSegment,
                        UserImagePath = i.UserProfile.ImageFileId,
                        UserEntityOwner = i.CreatedById == userId
                    }
                })
                .OrderByDescending(i => i.LastModified)
                .Skip(skip)
                .Take(take);

            return await itemsQuery.ToListAsync();
        }

        public async Task<List<ItemViewModel>> GetUserItemsByUserId(int skip, int take, string userId, int itemsCount, int width = 320)
        {
            var displayDateTimeFormat = _config["Application:DisplayDateTimeFormat"];

            var items = await _context.Items
                .Where(i => !i.IsDeleted && i.CreatedById == userId)
                .Where(i => i.File != null && i.File.FileDetails != null)
                .AsNoTracking()
                .Select(i => new ItemViewModel()
                {
                    CommentMostReply = i.Comments.Where(c => !c.IsDeleted)
                        .OrderByDescending(c => c.ChildCount).ThenByDescending(c => c.CreatedDate)
                        .Select(c => new CommentViewModel()
                        {
                            ChildCount = c.ChildCount,
                            Content = c.Content,
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
                    TotalCount = itemsCount,
                    UserProfile = new UserProfileEntityViewModel()
                    {
                        Name = i.UserProfile.Name,
                        UrlSegment = i.UserProfile.UrlSegment,
                        UserImagePath = i.UserProfile.ImageFileId,
                        UserEntityOwner = i.CreatedById == userId
                    }
                })
                .OrderByDescending(i => i.LastModified)
                .Skip(skip).Take(take)
                .ToListAsync();

            return items;
        }

        public async Task<List<ItemViewModel>> GetItemsRelatedToItemTagsByItemId(int skip, int take, string itemId, string userId, int width = 320)
        {

            var itemTagIds = await _context.Items.Where(i => !i.IsDeleted && i.ID == itemId)
                    .Select(i => new
                    {
                        ids = i.ItemTags.Where(it => !it.IsDeleted && it.Display).Select(it => it.TagID)
                    }).FirstOrDefaultAsync();

            var displayDateTimeFormat = _config["Application:DisplayDateTimeFormat"];

            var items = base.Query(i => !i.IsDeleted && i.ID != itemId && i.File != null && i.File.FileDetails != null)
                .AsNoTracking()
                .Include(i => i.ItemTags)
                .Where(i => i.ItemTags.Where(it => itemTagIds.ids.Contains(it.TagID)).Any())
                .OrderByDescending(i => i.ItemTags.Where(it => itemTagIds.ids.Contains(it.TagID)).Count())
                .ThenByDescending(i => i.CreatedDate)
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
                    //TotalCount = itemsCount,
                    UserProfile = new UserProfileEntityViewModel()
                    {
                        Name = i.UserProfile.Name,
                        UrlSegment = i.UserProfile.UrlSegment,
                        UserImagePath = i.UserProfile.ImageFileId,
                        UserEntityOwner = i.CreatedById == userId
                    }
                })
                .Skip(skip)
                .Take(take)
                .ToList();

            return items;
        }

        public async Task<List<ItemViewModel>> GetSearchItems(int skip, int take, SearchRequestModel parsedQuery, List<ItemTagsCountModel> matchingItemTags, IEnumerable<string> idsOfTagsMatchingItems, string userId, int itemsCount, int width = 320)
        {
            var displayDateTimeFormat = _config["Application:DisplayDateTimeFormat"];

            var items =
             await base.Query(i => !i.IsDeleted && i.File != null && i.File.FileDetails != null &&
             //(SearchItems(i, parsedQuery, idsOfTagsMatchingItems) || idsOfTagsMatchingItems.Any(itm => itm == i.ID))
             (parsedQuery.QueryCollection.Contains(i.Title.Trim().ToLower())
                || parsedQuery.QueryCollection.Contains(i.Description.Trim().ToLower())
                || (parsedQuery.QueryCollection.Count() == 1
                    && (i.Title.Trim().ToLower().Contains(parsedQuery.QueryCollection.First())
                    || i.Description.Trim().ToLower().Contains(parsedQuery.QueryCollection.First())))
                    || idsOfTagsMatchingItems.Any(ids => ids == i.ID)

            ))
                        .AsNoTracking().Include(i => i.ItemTags)
                        //.OrderByDescending(i => matchingItemTags.FirstOrDefault(mit => mit != null && mit.ItemId == i.ID) != null ? matchingItemTags.FirstOrDefault(mit => mit.ItemId == i.ID).MatchingTagsCount : 0)
                        //.ThenByDescending(i =>
                        //	parsedQuery.QueryCollection.Count(q => i.Title.ToLower().Contains(q) || i.Description.ToLower().Contains(q)))
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
                        }).FirstOrDefault(),
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
                            TotalCount = itemsCount,
                            UserProfile = new UserProfileEntityViewModel()
                            {
                                Name = i.UserProfile.Name,
                                UrlSegment = i.UserProfile.UrlSegment,
                                UserImagePath = i.UserProfile.ImageFileId,
                                UserEntityOwner = i.CreatedById == userId
                            }
                        })
                        .Skip(skip).Take(take)
                        .ToListAsync();

            return items;
        }

        private static bool SearchItems(Item i, SearchRequestModel parsedQuery, IEnumerable<string> idsOfTagsMatchingItems)
        {
            if (!string.IsNullOrWhiteSpace(parsedQuery.QueryCollection.FirstOrDefault()))
            {
                return !i.IsDeleted &&
                (parsedQuery.QueryCollection.Contains(i.Title.Trim().ToLower())
                || parsedQuery.QueryCollection.Contains(i.Description.Trim().ToLower())
                || (parsedQuery.QueryCollection.Count() == 1
                    && (i.Title.Trim().ToLower().Contains(parsedQuery.QueryCollection.First())
                    || i.Description.Trim().ToLower().Contains(parsedQuery.QueryCollection.First()))));
            }
            else
            {
                return idsOfTagsMatchingItems.Any(ids => ids == i.ID);
                //.Contains(i.ID);
            }
        }

        public async Task<List<ItemViewModel>> GetUserFavouritesItemsIdsByUserId(int skip, int take, string userId, int itemsCount, int width = 320)
        {
            var displayDateTimeFormat = _config["Application:DisplayDateTimeFormat"];

            var items = await _context.Favourites
                .Where(f => f != null && !f.IsDeleted && f.CreatedById == userId && f.Item.File != null && f.Item.File.FileDetails != null)
                .AsNoTracking()
                .OrderByDescending(f => f.OrderNumber)
                .Include(f => f.Item)
                .Select(f => new ItemViewModel()
                {
                    CommentMostReply = f.Item.Comments.Where(c => !c.IsDeleted)
                        .OrderByDescending(c => c.ChildCount).ThenByDescending(c => c.CreatedDate)
                        .Select(c => new CommentViewModel()
                        {
                            ChildCount = c.ChildCount,
                            Content = c.Content,
                            //CreatedById = c.CreatedById,
                            //CreatedDate = c.CreatedDate.ToString(displayDateTimeFormat),
                            CreatedDateD = c.CreatedDate,
                            DateFormat = displayDateTimeFormat,
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
                        }).FirstOrDefault(),
                    CommentsCount = f.Item.CommentsCount,
                    //CreatedDate = i.Item.CreatedDate.ToString(displayDateTimeFormat),
                    DateFormat = displayDateTimeFormat,
                    CreatedDateD = f.Item.CreatedDate,
                    UserFavourite = f.Item.Favourites.Where(f => !f.IsDeleted && f.CreatedById == userId).Any(),
                    FavouritesCount = f.Item.FavouritesCount,
                    ItemFilePath = f.Item.FileID,
                    FileDetails = new FileDetailsViewModel()
                    {
                        Color = f.Item.File.FileDetails.Color,
                        Height = f.Item.File.FileDetails.Height,
                        Width = f.Item.File.FileDetails.Width,
                        ImageAnalysisRaw = f.Item.File.FileDetails.ImageAnalysis
                    },
                    Id = f.Item.ID,
                    Tags = f.Item.ItemTags.Where(it => !it.IsDeleted && it.Display).Select(it => it.Tag.Name),
                    LastModified = f.LastModified,
                    UserLiked = f.Item.Likes.Where(l => !l.IsDeleted && l.CreatedById == userId).Any(),
                    LikesCount = f.Item.LikesCount,
                    Title = f.Item.Title,
                    Description = f.Item.Description,
                    TotalCount = itemsCount,
                    UserProfile = new UserProfileEntityViewModel()
                    {
                        Name = f.Item.UserProfile.Name,
                        UrlSegment = f.Item.UserProfile.UrlSegment,
                        UserImagePath = f.Item.UserProfile.ImageFileId,
                        UserEntityOwner = f.Item.CreatedById == userId
                    }
                })
                .Skip(skip)
                .Take(take)
                .ToListAsync();




            //await (
            //	from item in _context.Items
            //	join fav in _context.Favourites on item.ID equals fav.EntityId
            //	where !item.IsDeleted && !fav.IsDeleted && fav.CreatedById == userId
            //	orderby fav.OrderNumber descending
            //	select item
            //	)
            //	.Include(i => i.ItemTags)
            //	.Include(i => i.Comments)
            //	.Skip(skip).Take(take)
            //	.ToListAsync();

            return items;
        }

        public async Task<int> CountAllItems()
        {
            var itemsCount = await base.Query(i => !i.IsDeleted).CountAsync();

            if (itemsCount == 0)
            {
                //ThrowItemsNotFoundException();
            }

            return itemsCount;
        }

        public async Task<int> CountUserItemsByUserId(string userId)
        {
            var itemsCount = await _context
                .Items.Where(i => !i.IsDeleted && i.CreatedById == userId).AsNoTracking()
                .CountAsync();

            if (itemsCount == 0)
            {
                //ThrowUserItemsNotFoundForUserId(userId);
            }

            return itemsCount;
        }

        public async Task<int> CountSearchItems(SearchRequestModel parsedQuery, List<ItemTagsCountModel> matchingItemTags, IEnumerable<string> idsOfTagsMatchingItems)
        {
            var count = await base.Query(i => !i.IsDeleted &&
            //(SearchItems(i, parsedQuery, idsOfTagsMatchingItems)
            //|| idsOfTagsMatchingItems.Any(ids => ids == i.ID)
            (parsedQuery.QueryCollection.Contains(i.Title.Trim().ToLower())
                || parsedQuery.QueryCollection.Contains(i.Description.Trim().ToLower())
                || (parsedQuery.QueryCollection.Count() == 1
                    && (i.Title.Trim().ToLower().Contains(parsedQuery.QueryCollection.First())
                    || i.Description.Trim().ToLower().Contains(parsedQuery.QueryCollection.First())))
                    || idsOfTagsMatchingItems.Any(ids => ids == i.ID)

            ))
                                    .AsNoTracking()
                                    .Include(i => i.ItemTags)
                                    //.OrderByDescending(i => matchingItemTags.FirstOrDefault(mit => mit.ItemId == i.ID) != null ? matchingItemTags.FirstOrDefault(mit => mit.ItemId == i.ID).MatchingTagsCount : 0)
                                    //.ThenByDescending(i =>
                                    //	parsedQuery.QueryCollection.Count(q => i.Title.ToLower().Contains(q) || i.Description.ToLower().Contains(q)))
                                    .CountAsync();

            return count;
        }

        public async Task<int> CountUserFavouriteItemsByUserId(string userId)
        {
            //var itemsCount =

            var itemsCount = await _context.Favourites.Where(f => !f.IsDeleted && f.CreatedById == userId).AsNoTracking()
                                    .CountAsync();

            //await (from fav in _context.Favourites
            //					join item in _context.Items on fav.EntityId equals item.ID
            //					where !fav.IsDeleted && !item.IsDeleted && fav.CreatedById == userId
            //					select fav
            //					).CountAsync();
            if (itemsCount == 0)
            {
                //throw new UserFavouritesItemsNotFoundException(
                //	HttpStatusCode.NoContent, Errors.ErrorFavouritesItemsNotFound);
            }

            return itemsCount;
        }

        public async Task<int> CountUserItemsFavouritesByUserId(string userId)
        {
            return await base.Query(i => !i.IsDeleted && i.CreatedById == userId, false)
                        .Select(i => i.FavouritesCount)
                        .SumAsync();
        }

        public async Task<int> CountUserItemsLikesByUserId(string userId)
        {
            return await base.Query(i => !i.IsDeleted && i.CreatedById == userId, false)
                        .Select(i => i.LikesCount)
                        .SumAsync();
        }

        public async Task<int> CountUserItemsCommentsByUserId(string userId)
        {
            return await base.Query(i => !i.IsDeleted && i.CreatedById == userId, false)
                        .Select(i => i.CommentsCount)
                        .SumAsync();
        }


        private static void ThrowUserItemsNotFoundForUserId(string userId)
        {
            //throw new HttpStatusCodeException(HttpStatusCode.NoContent,
            //				   null,
            //				   Errors.ErrorItemsNotFound,
            //				   null,
            //				   $"User Items not found for userId: {userId}");
        }

        private static void ThrowItemsNotFoundException()
        {
            //throw new HttpStatusCodeException(HttpStatusCode.NoContent,
            //				   null,
            //				   Errors.ErrorItemsNotFound,
            //				   null,
            //				   $"Items not found");
        }

        private static void ThrowItemsNotFoundForRelatedItemTagsException(IEnumerable<string> itemTagIds)
        {
            //throw new HttpStatusCodeException(HttpStatusCode.NoContent,
            //			   null,
            //			   Errors.ErrorItemNotFound,
            //			   null,
            //			   $"Items not found for related itemTagsIds: {itemTagIds}");
        }

        private static void ThrowItemNotFoundException(string itemId)
        {
            //throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
            //				   null,
            //				   Errors.ErrorItemNotFound,
            //				   null,
            //				   $"Item not found for itemId: '{itemId}'.");
        }
    }
}
