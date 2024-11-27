using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quantum.AuthorizationServer.Models;
using Quantum.AuthorizationServer.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Entities.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using Quantum.Utility.Services.Contracts;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services
{
	public class UserProfileService : IUserProfileService
    {
        private ILogger<UserProfileService> _logger;
        private IFileRepository _fileRepo;
        private IUserProfileRepository _userProfileRepo;
        private UserManager<IdentityUser> _userMgr;
        private IUtilityService _utliServ;
        private IFileTypeRepository _fileTypeRepo;
        private IConfigurationRoot _config;
		private IMappingFileService _mappingFileServe;
		private IMapper _mapper;

		public UserProfileService(
            ILogger<UserProfileService> logger,
            IFileRepository fileRepo,
            IUserProfileRepository userProfileRepo,
            UserManager<IdentityUser> userMgr,
            IUtilityService utliServ,
			IFileTypeRepository fileTypeRepo,
			IConfigurationRoot config,
			IMappingFileService mappingFileServe,
			IMapper mapper
		)
        {
            _logger = logger;
            _fileRepo = fileRepo;
            _userProfileRepo = userProfileRepo;
            _userMgr = userMgr;
            _utliServ = utliServ;
			_fileTypeRepo = fileTypeRepo;
            _config = config;
			_mappingFileServe = mappingFileServe;
			_mapper = mapper;
        }

        public async Task<bool> UrlSegmentExists(string segmentName)
        {
            var trim = Regex.Replace(segmentName.ToLower(), @"\s+", "");

            var exists = await _userProfileRepo.UrlNameAlreadyExists(trim);

            return exists;
        }

        public async Task<(IdentityResult IdentityResult, IdentityUser User)> CreateUser(RegisterModel model)
        {

			IdentityUser user = _mapper.Map<RegisterModel, IdentityUser>(model);

			IdentityResult result = await _userMgr.CreateAsync(user, model.Password);

            return (IdentityResult: result, User: user);
        }

        public async Task CreateUserProfile(RegisterModel model, IdentityUser user)
        {

			var userProfile = _mapper.Map<IdentityUser, UserProfile>(user);

			userProfile = _mapper.Map<RegisterModel, UserProfile>(model);

			await _userProfileRepo.Insert(userProfile, user);
        }

        public async Task CreateUserClaims(IdentityUser user, RegisterModel model)
        {
            var claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, model.Name),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                };

            await _userMgr.AddClaimsAsync(user, claims);
        }

        public async Task<UserModel> GetUserProfileByEmail(string email, bool getProfileImage = false)
        {
            var user = await _userMgr.FindByEmailAsync(email);

            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                           "Oops something went wrong. Please try again later.",
                           Errors.GeneralError,
                           null,
                           $"User cannot be found for email: '{email}'.");
            }

            var userProfile = await _userProfileRepo.GetByUserId(user.Id);

            if (userProfile == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                         "Oops something went wrong.  Please try again later.",
                         Errors.GeneralError,
                         null,
                         $"UserProfile cannot be found for UserId'{user.Id}'.");
            }

			if (userProfile.ImageFileId == null)
			{
				getProfileImage = false;
			}

            var imagePath = string.Empty;

            if (getProfileImage)
            {
				string extension = await _fileRepo.GetFileExtensionById(userProfile.ImageFileId);
				var imagesFolder = $"{_config["Application:ImagesPath"]}/{_config["Application:ImageSize:320"]}";
				imagePath = $"{imagesFolder}/{userProfile.ImageFileId}.{extension}";
			}

			var userModel = await _mappingFileServe.AddUserProfileserModelMappings(userProfile, imagePath, user.EmailConfirmed);

			return userModel;
        }
    }
}
