using AutoMapper;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services
{
    public class MappingItemsService : IMappingItemsService
	{
		private IConfiguration _config;
        private ILogger<MappingItemsService> _logger;
        private IMapper _mapper;
        private IFileRepository _fileRepo;
		private IUserProfileRepository _userProfileRepo;
        private IUtilityService _utilServ;
		private IItemRepository _itemRepo;
		private ICommentRepository _commentRepo;
		private IMappingCommentService _mappingCommentServ;
		private IFavouriteRepository _favouriteRepo;
		private ILikeRepository _likeRepo;
		private IImageService _imageServ;

		private QDbContext _context;

		private readonly IServiceScopeFactory _serviceScopeFactory;
		public IServiceProvider Services { get; }
		public IBackgroundTaskQueue Queue { get; }

		public MappingItemsService(
			IConfiguration config,
			IFileRepository fileRepo,
			IUserProfileRepository userProfileRepo,
			IMapper mapper,
			ILogger<MappingItemsService> logger,
            IUtilityService utilServ,
			IItemRepository itemRepo,
			ICommentRepository commentRepo,
			IMappingCommentService mappingCommentServ,
			IFavouriteRepository favouriteRepo,
			ILikeRepository likeRepo,
			IImageService imageServ,
			QDbContext context,
			IServiceScopeFactory serviceScopeFactory,
			IServiceProvider services,
			IBackgroundTaskQueue queue
			)
		{
			_config = config;
			_fileRepo = fileRepo;
			_userProfileRepo = userProfileRepo;
			_mapper = mapper;
			_logger = logger;
            _utilServ = utilServ;
			_itemRepo = itemRepo;
			_commentRepo = commentRepo;
			_mappingCommentServ = mappingCommentServ;
			_favouriteRepo = favouriteRepo;
			_likeRepo = likeRepo;
			_imageServ = imageServ;
			_context = context;
			_serviceScopeFactory = serviceScopeFactory;
			Services = services;
			Queue = queue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="itemsCount"></param>
		/// <param name="tags"></param>
		/// <param name="filterTags"></param>
		/// <param name="userId"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public async Task<ItemViewModel> MapItemViewModelFromItem(Item item, int itemsCount,
			IEnumerable<Tag> tags, bool filterTags = false, string userId = null, int width = 320)
		{


			if (item.ItemTags.Count > 0 && filterTags)
			{
				tags = tags.Where(t => item.ItemTags.Select(it => it.TagID).Contains(t.ID));
			}
			else if (item.ItemTags.Count == 0)
			{
				tags = null;
			}

			string[] tagNames = new string[] { };

			if (tags != null)
			{
				tagNames = tags.Select(t => t.Name.ToLower())
							   .Distinct()
							   .ToArray();
			}


			var userProfile = await _userProfileRepo.GetByUserId(item.CreatedById);

			var userProfileImagePath = string.Empty;

			if (userProfile.ImageFileId != null)
			{
				userProfileImagePath = userProfile.ImageFileId;
			}


			var fileDetails = new FileDetails();
			if (item.FileID != null)
			{
				fileDetails = await _fileRepo.GetFileDetailsByFileId(item.FileID);
			}
			FileDetailsViewModel fileDetailsModel = _mapper.Map<FileDetailsViewModel>(fileDetails);
			var itemFilePath = item.FileID;
			var userLiked = await _likeRepo.IsUserLiked(userId, item.ID);
			var userFavourite = await _favouriteRepo.IsUserFavourite(userId, item.ID);


			var viewItem = _mapper.Map<ItemViewModel>(item);

			viewItem.UserProfile = new UserProfileEntityViewModel()
			{
				UrlSegment = userProfile.UrlSegment,
				UserImagePath = userProfileImagePath,
				Name = userProfile.Name,
				UserEntityOwner = userId == item.CreatedById

		};

			var displayDateTimeFormat = _config["Application:DisplayDateTimeFormat"];


            viewItem.Tags = tagNames;
			viewItem.ItemFilePath = item.FileID;
			//viewItem.CreatedDate = item.CreatedDate.ToString(displayDateTimeFormat);
			viewItem.CreatedDateD = item.CreatedDate;
			viewItem.DateFormat = displayDateTimeFormat;
            viewItem.FileDetails = fileDetailsModel;
			viewItem.UserLiked = await _likeRepo.IsUserLiked(userId, item.ID);
			viewItem.UserFavourite = await _favouriteRepo.IsUserFavourite(userId, item.ID);

			viewItem.CommentMostReply = await GetMostCommentsViewModels(item, userId);

			viewItem.TotalCount = itemsCount;

			return await Task.FromResult(viewItem);
		}

		private async Task<CommentViewModel> GetMostCommentsViewModels(Item item, string userId)
		{
			var commentsList = new List<Comment>();

			var commentMostReplied = item.Comments
				.Where(c => !c.IsDeleted).OrderByDescending(c => c.ChildCount)
				.ThenByDescending(c => c.CreatedDate).FirstOrDefault();

			//if (commentMostReplied != null)
			//	commentsList.Add(commentMostReplied);
			CommentViewModel viewComments = null;

            if (commentsList.Count() != 0)
            {
				var commentViewMapping = _mappingCommentServ.MapCommentViewModelFromComment(commentMostReplied, userId).GetAwaiter().GetResult();

				//	var commentViewMapping = commentsList.Select(co => _mappingCommentServ.MapCommentViewModelFromComment(co, userId).GetAwaiter().GetResult())
				//		.Where(v => v != null)
				//		.ToArray();

				viewComments = await Task.FromResult(commentViewMapping);
            }

            return viewComments;
		}

		public async Task<Item> MapItemFromItemModel(ItemCreateModel model, ImageAnalysis imageAnalysis, string fileId, string userProfileId)
		{
			GetTitleAndDescriptionFromCaptionOrTag(imageAnalysis, out string description, out string title);

			var item = _mapper.Map<ItemCreateModel, Item>(model);

			item.Title = title;
			item.Description = description;
			item.FileID = fileId;
			item.UserProfileId = userProfileId;

			return await Task.FromResult(item);
		}

		private void GetTitleAndDescriptionFromCaptionOrTag(ImageAnalysis imageAnalysis, out string description, out string title)
		{
			var captionLenght = _config.GetAsInteger("Application:CaptionLenght", 25);
			description = GetAnalyzeCaption(imageAnalysis.Description);
			title = description;
			if (title.Count() > captionLenght)
			{
				title = title.Substring(0, captionLenght);
			}
		}


		private string GetAnalyzeCaption(ImageDescriptionDetails description)
		{
			var caption = description.Captions.OrderByDescending(ca => ca.Confidence).FirstOrDefault();

			if(caption != null)
			{
				return caption.Text;
			}

			var tag = description.Tags.FirstOrDefault();

			if(tag != null)
			{
				return tag;
			}

			return string.Empty;
			
		}
	}
}
