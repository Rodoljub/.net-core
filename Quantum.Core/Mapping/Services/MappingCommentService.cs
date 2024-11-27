using AutoMapper;
using Microsoft.Extensions.Configuration;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Models.ReadModels;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Extensions;
using Quantum.Utility.Services.Contracts;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services
{
    public class MappingCommentService : IMappingCommentService
	{
		private IMapper _mapper;
		private IConfiguration _config;
		private IUtilityService _utilServ;
		private IUserProfileRepository _userProfileRepo;
		private IFileRepository _fileRepo;
		private ILikeRepository _likeRepo;

		public MappingCommentService(
			IMapper mapper,
			IConfiguration config,
			IUtilityService utilServ,
			IUserProfileRepository userProfileRepo,
			IFileRepository fileRepo,
			ILikeRepository likeRepo
		)
		{
			_mapper = mapper;
			_config = config;
			_utilServ = utilServ;
			_userProfileRepo = userProfileRepo;
			_fileRepo = fileRepo;
			_likeRepo = likeRepo;
		}

		public async Task<CommentViewModel> MapCommentViewModelFromComment(Comment comment, string userId)
		{
			var commentViewModel = _mapper.Map< Comment, CommentViewModel >(comment);
			var userProfile = await _userProfileRepo.GetByUserId(comment.CreatedById);

			var userProfileImagePath = string.Empty;
			if (userProfile.ImageFileId != null)
			{
				userProfileImagePath = userProfile.ImageFileId;
			}

			var userCommentOwner = false;
			if (userId == comment.CreatedById)
				userCommentOwner = true;

			bool userLiked = await _likeRepo.IsUserLiked(userId, comment.ID);
			//string createdDate = _utilServ.TimeAgo(comment.CreatedDate);
			//string createdDate = comment.CreatedDate.ToString("MM/dd/yyyy HH:mm:ss");
			commentViewModel.UserProfile = new UserProfileEntityViewModel()
			{
				UrlSegment = userProfile.UrlSegment,
				Name = userProfile.Name,
				UserImagePath = userProfileImagePath,
				UserEntityOwner = userCommentOwner
			};
			//commentViewModel.UrlSegment = userProfile.UrlSegment;
			//commentViewModel.UserName = userProfile.Name;
			//commentViewModel.UserImage = userProfileImagePath;

			//commentViewModel.UserCommentOwner = userCommentOwner;
				//userId == comment.CreatedById ? true : false;
			commentViewModel.UserLiked = userLiked;
			//commentViewModel.CreatedDate = createdDate;
			commentViewModel.CreatedDateD = comment.CreatedDate;
			commentViewModel.DateFormat = "MM/dd/yyyy HH:mm:ss";


            return commentViewModel;
		}

		public async Task<Comment> MapCommentFromCommentModel(CommentModel commentModel, string clrTypeId, string userProfileId)
		{
			var comment = _mapper.Map<CommentModel, Comment>(commentModel);
			comment.ParentTypeID = clrTypeId;
			comment.UserProfileId = userProfileId;
			return await Task.FromResult(comment);
		}
	}
}
