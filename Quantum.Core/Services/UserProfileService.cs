using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Core.Models.Auth;
using Quantum.Core.Models.UserProfile;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using Quantum.Integration.External.Services.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using Quantum.Utility.Infrastructure.Exceptions;
using Quantum.Utility.Services.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using File = Quantum.Data.Entities.File;

namespace Quantum.Core.Services
{
    public class UserProfileService : IUserProfileService
    {
        private IUtilityService _utilityServ;
        private IFileService _fileServ;
        private IFileRepository _fileRepo;
        private IFileDetailsRepository _fileDetailsRepo;
        private IFileTypeRepository _fileTypeRepo;
        private UserManager<IdentityUser> _userMgr;
        private IUserManagerService _userMgrServ;
        private IUserProfileRepository _userProfileRepo;
        private IConfiguration _config;
        private IMappingUserProfileService _mappingUserProfileServ;
        private IMappingFileService _mappingFileServ;
        private IMapper _mapper;
        private IImageService _imageServ;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IBlobsStorageService _blobService;

        public IServiceProvider Services { get; }
        public IBackgroundTaskQueue Queue { get; }

        public UserProfileService(
            IUtilityService utilityServ,
            IConfiguration config,
            IFileService fileServ,
            IFileRepository fileRepo,
            IFileDetailsRepository fileDetailsRepo,
            IFileTypeRepository fileTypeRepo,
            UserManager<IdentityUser> userMgr,
            IUserManagerService userMgrServ,
            IUserProfileRepository userProfileRepo,
            IMappingUserProfileService mappingUserProfileServ,
            IMappingFileService mappingFileServ,
            IMapper mapper,
            IImageService imageServ,
            IServiceScopeFactory serviceScopeFactory,
            IBlobsStorageService blobService,
            IServiceProvider services,
            IBackgroundTaskQueue queue
            )
        {
            _utilityServ = utilityServ ?? throw new ArgumentNullException(nameof(utilityServ));
            _fileServ = fileServ ?? throw new ArgumentNullException(nameof(fileServ));
            _fileRepo = fileRepo ?? throw new ArgumentNullException(nameof(fileRepo));
            _fileDetailsRepo = fileDetailsRepo ?? throw new ArgumentNullException(nameof(fileDetailsRepo));
            _fileTypeRepo = fileTypeRepo ?? throw new ArgumentNullException(nameof(fileTypeRepo));
            _userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
            _userMgrServ = userMgrServ ?? throw new ArgumentNullException(nameof(userMgrServ));
            _userProfileRepo = userProfileRepo ?? throw new ArgumentNullException(nameof(userProfileRepo));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _mappingUserProfileServ = mappingUserProfileServ ?? throw new ArgumentNullException(nameof(mappingUserProfileServ));
            _mappingFileServ = mappingFileServ ?? throw new ArgumentNullException(nameof(mappingFileServ));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _imageServ = imageServ ?? throw new ArgumentNullException(nameof(imageServ));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        public async Task CreateUserProfile(RegisterModel model, IdentityUser user)
        {
            var userProfile = _mapper.Map<IdentityUser, UserProfile>(user);

            userProfile = _mapper.Map<RegisterModel, UserProfile>(model);
            userProfile.UrlSegment = await CreateRandomUrlSegment(user.Email);
            userProfile.TermsAndConditions = true;

            await _userProfileRepo.Insert(userProfile, user);
        }

        public async Task<UpdatedProfileModel> UpdateProfile(UserProfileModel model, IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);

            var userProfile = await _userProfileRepo.GetByUserId(user?.Id);

            if (userProfile == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                 null,
                 Errors.ErrorUserProfileNotFound,
                 null,
                 $"User Profile not found for userId: {user.Id}");
            }

            var profileImageFileId = string.Empty;
            bool hasImage = false;
            var analyzingImage = new AnalyzingImageViewModel();

            profileImageFileId = userProfile.ImageFileId;

            if (model.UserImage != null)
            {
                if (profileImageFileId != null)
                {
                    hasImage = true;
                }

                analyzingImage = await SaveProfileImageFile(user, model.UserImage, hasImage, profileImageFileId);
            }

            //userProfile =  await _mappingUserProfileServ.MapUserProfileFromUserProfile(userProfile, model.Name, profileImageFileId);
            userProfile.Name = model.Name;
            await _userProfileRepo.Update(userProfile, user, save: true);

            var updatedProfile = new UpdatedProfileModel()
            {
                Name = userProfile.Name,
                AnalyzingImageViewModel = analyzingImage
            };

            return updatedProfile;


        }

        private async Task<AnalyzingImageViewModel> SaveProfileImageFile(IdentityUser user, string base64StringRaw, bool hasImage = false, string profileImageFileId = "")
        {
            var base64ImageStringArray = _imageServ.GetBase64ImageArrayFromBase64StringRaw(base64StringRaw, user.Id);

            var userImageByteArray = Convert.FromBase64String(base64ImageStringArray[1]);

            _imageServ.ValidateImageFromByteArray(userImageByteArray, user.Id);

            ValidateFileMaxLength(userImageByteArray);

            await ValidUpdateQuota(user?.Id);



            string fileExstension = base64ImageStringArray[0].TrimStart("data:image/".ToArray());

            string fileContentType = base64ImageStringArray[0].TrimStart("data:".ToArray());

            var userImageFile = await SaveUserProfileImageFile(userImageByteArray, fileExstension, user, hasImage, profileImageFileId);

            var isSuccessUploaded = await SaveImageOnBlobStorage(userImageByteArray, userImageFile, fileContentType);

            if (!isSuccessUploaded)
            {
                await _fileRepo.Delete(userImageFile.ID, user);
                throw new GeneralErrorException(HttpStatusCode.BadRequest, Errors.GeneralError);
            }
            //_imageServ.SaveImagesOnWebServer(userImageByteArray, userImageFile, fileExstension);

            int width80 = _config.GetAsInteger($"Application:ImageSize:80", 80);
            var base64 = _utilityServ.ByteArrayImageToBase64Image(userImageByteArray, fileExstension, true, width80);

            var analyzingImage = new AnalyzingImageViewModel()
            {
                FileId = userImageFile.ID,
                FileExtension = userImageFile.Extension,
                Image = base64
            };
            return analyzingImage;
            //return userImageFile.ID;
        }

        private async Task<File> SaveUserProfileImageFile(byte[] userImageByteArray, string fileExstension, IdentityUser user, bool hasImage, string profileImageFileId)
        {
            var fileType = await _fileTypeRepo.GetFileTypeByName(FileTypes.Images.ProfileImage);

            var fileName = $"{user.UserName}_{FileTypes.Images.ProfileImage}.{fileExstension}";

            Data.Entities.File file = new File
            {
                Name = fileName,
                Extension = fileExstension,
                TypeId = fileType.ID
            };
            //await _mappingFileServ.MapFileFromByteArray(userImageByteArray, fileExstension, fileType.ID, fileName);

            //if (hasImage)
            //{
            //	File existingFile = await _fileRepo.GetById(profileImageFileId);

            //	existingFile = _mapper.Map(file, existingFile);

            //	await _fileRepo.UpdateFile(existingFile, user);

            //	return existingFile;
            //}
            //else
            //{
            return await _fileRepo.InsertFile(file, user);
            //}
        }

        public async Task<bool> ValidUpdateQuota(string userId)
        {
            var maxUpdateNumber = _config.GetAsInteger("User:Profile:MaxUpdateNumber", 30);
            var lastMinutes = DateTime.UtcNow.AddMinutes(_config.GetAsInteger("User:Profile:LastMinutes", -30));
            var numberOfUpdates = await _userProfileRepo.Query(up => up.CreatedById == userId && up.CreatedDate > lastMinutes)
                .AsNoTracking()
                .CountAsync();

            if (numberOfUpdates < maxUpdateNumber)
            {
                return true;
            }

            throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                 null,
                 Errors.ErrorUserProfileMaxUpdateInTime,
                 null,
                 $"User: '{userId}' max update profile in time");
        }

        private void ValidateFileMaxLength(byte[] byteArray)
        {
            var maxLength = _config.GetAsInteger("Application:AzureBlob:MaxFileSize");
            var maxLengthMb = _config["Application:AzureBlob:MaxFileSizeMb"];
            if (byteArray.Length > maxLength)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                   null,
                   Errors.ErrorFileIsTooLarge,
                   null,
                   $"File is too large. Max is {maxLengthMb}");
            }
        }

        private async Task<bool> SaveImageOnBlobStorage(byte[] imageByte, File file, string fileContentType)
        {
            var containerName = _config["Application:AzureBlob:ContainerImagesForAnalysisName"];
            var fileName = $"{file.ID}.{file.Extension}";
            var image = Image.Load<Rgba32>(new MemoryStream(imageByte));
            var imageUpdated = _utilityServ.UpdateImageOrientation(image);
            var imageFormat = _utilityServ.GetImageFormat(file.Extension);
            var imageStream = _utilityServ.ImageToStream(imageUpdated, imageFormat);
            var contentType = fileContentType;
            var isSuccessUpload = await _blobService.UploadBlobImageToContainer(containerName, fileName, imageStream, contentType);
            return isSuccessUpload;

        }

        public async Task<UserProfileViewModel> GetUserProfileViewModel(IIdentity identity)
        {
            var user = await _utilityServ.GetAuthUser(identity);

            var userProfile = await _userProfileRepo.GetByUserIdAsNoTracking(user.Id);

            var profileImageAnalyzingFile = await _fileRepo.GetProfileImageAnalyzingByUserId(user.Id);

            var analyzingImage = new AnalyzingImageViewModel();

            if (profileImageAnalyzingFile != null)
            {
                string containerName = _config["Application:AzureBlob:ContainerImagesForAnalysisName"];

                string fileName = $"{profileImageAnalyzingFile.ID}.{profileImageAnalyzingFile.Extension}";
                Stream stream = await _blobService.GetBlobStream(containerName, fileName);
                stream.Position = 0;
                var newStream = new System.IO.MemoryStream();
                stream.CopyTo(newStream);
                newStream.Position = 0;
                //var byteArray = newStream.ToArray();

                int width80 = _config.GetAsInteger($"Application:ImageSize:80", 80);
                var byteArray = _utilityServ.StreamToByteArray(newStream, (int)newStream.Length);
                var base64 = _utilityServ.ByteArrayImageToBase64Image(byteArray, profileImageAnalyzingFile.Extension, true, width80);



                analyzingImage = new AnalyzingImageViewModel()
                {
                    FileId = profileImageAnalyzingFile.ID,
                    FileExtension = profileImageAnalyzingFile.Extension,
                    Image = base64
                };
            }

            var userProfileViewModel = await MapUserProfileViewModel(userProfile, analyzingImage);

            return userProfileViewModel;
        }
        public async Task<UserProfileViewModel> GetUserProfileViewModelByUrlSegment(string urlSegment)
        {
            var userProfile = await _userProfileRepo.GetUserProfileByUrlSegment(urlSegment);

            return await MapUserProfileViewModel(userProfile);
        }

        public async Task<UserProfileViewModel> GetUserProfileViewModelByEmail(string email)
        {
            var userProfile = await _userProfileRepo.GetUserProfileByEmail(email);

            return await MapUserProfileViewModel(userProfile);
        }

        private async Task<UserProfileViewModel> MapUserProfileViewModel(UserProfile userProfile, AnalyzingImageViewModel analyzingImage = null)
        {

            string userImagePath = userProfile.ImageFileId;

            var userProfileViewModel = await _mappingUserProfileServ.MapUserProfileViewModelFromUserProfile(userProfile, userImagePath);
            userProfileViewModel.AnalyzingImageViewModel = analyzingImage;
            return userProfileViewModel;
        }

        public async Task<string> SetUserProfileCounters()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                var scopedItemsListRepo = scopedServices.GetRequiredService<IItemsListRepository>();

                var userProfiles = await _userProfileRepo.Query(up => !up.IsDeleted)
                    .ToListAsync();

                foreach (var userProfile in userProfiles)
                {
                    var userId = userProfile.CreatedById;

                    var imagesCount = await scopedItemsListRepo.CountUserItemsByUserId(userId);
                    userProfile.UploadsCount = imagesCount;

                    var commentsCount = await scopedItemsListRepo.CountUserItemsCommentsByUserId(userId);
                    userProfile.CommentsCount = commentsCount;

                    var favouritesCount = await scopedItemsListRepo.CountUserItemsFavouritesByUserId(userId);
                    userProfile.FavouritesCount = favouritesCount;

                    var likesCount = await scopedItemsListRepo.CountUserItemsLikesByUserId(userId);
                    userProfile.LikesCount = likesCount;

                    await _userProfileRepo.Update(userProfile, null, false);
                }

                await _userProfileRepo.Save();
            }

            return "saveProfileCounter";
        }

        public async Task<string> CreateRandomUrlSegment(string email)
        {

            var trim = $"{Regex.Replace(email.ToLower().Split(new char[] { '@' })[0], @"\s+", "")}";

            var alphanumericTrim = Regex.Replace(trim, "[^a-zA-Z0-9]", String.Empty);

            var random = new Random();
            var randomNumber = 0;
            for (int i = 0; i <= 100; i++)
            {
                randomNumber = random.Next(10000, 99999);
                var randomSegment = $"{alphanumericTrim}{randomNumber}";

                var isSegmentExist = await _userProfileRepo.UrlSegmentAlreadyExists(randomSegment);
                if (!isSegmentExist)
                {
                    break;
                }
                if (i == 100)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                        null,
                        Errors.GeneralError,
                        null);
                }
            }

            return $"{alphanumericTrim}{randomNumber}";
        }

        public async Task UpdateAnalysedProfileImage(File file, ImageAnalysis imageAnalysis)
        {
            IdentityUser user = await _userMgr.FindByIdAsync(file.CreatedById);

            var userProfile = await _userProfileRepo.GetByUserId(user.Id);

            await DeleteFileAndThrowExceptionIfImageForbidden(file, imageAnalysis, user);

            await MapAndInsertFileDetails(file, imageAnalysis, user);

            if (userProfile.ImageFileId == null)
            {
                userProfile.ImageFileId = file.ID;
                await _userProfileRepo.Update(userProfile, user);
            }
            else
            {
                var existingProfileImageFile = await _fileRepo.GetFileById(userProfile.ImageFileId);

                await _blobService.DeleteWebPJpegImages(existingProfileImageFile.ID);


                var containerName = _config["Application:AzureBlob:ContainerImagesForAnalysisName"];
                var fileName = $"{existingProfileImageFile.ID}.{existingProfileImageFile.Extension}";
                await _blobService.DeleteBlobFromContainer(containerName, fileName);

                await _fileRepo.Delete(existingProfileImageFile.ID, user);

                userProfile.ImageFileId = file.ID;
                await _userProfileRepo.Update(userProfile, user);

            }

            QueueSetNotificationAndEvent(file, user);

        }

        private void QueueSetNotificationAndEvent(File file, IdentityUser user)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var scopedNotificationService = scopedServices.GetRequiredService<INotificationService>();

                    await scopedNotificationService.ProfileImageReady(file, user);
                }
            });
        }

        private async Task MapAndInsertFileDetails(File file, ImageAnalysis analyzedImage, IdentityUser user)
        {
            var fileDetails = _mapper.Map<ImageAnalysis, FileDetails>(analyzedImage);

            fileDetails.ImageAnalysis = JsonConvert.SerializeObject(analyzedImage);

            var fileDetailsId = await _fileDetailsRepo.InsertFileDetails(fileDetails, user);

            file.FileDetailsId = fileDetailsId;

            await _fileRepo.Update(file, user);
        }

        private async Task DeleteFileAndThrowExceptionIfImageForbidden(File file, ImageAnalysis imageAnalysis, IdentityUser user)
        {
            if (imageAnalysis.Adult.IsAdultContent || imageAnalysis.Adult.IsRacyContent)
            {
                await _fileRepo.Delete(file.ID, user);

                var containerName = _config["Application:AzureBlob:ContainerImagesForAnalysisName"];
                var fileName = $"{file.ID}.{file.Extension}";

                await _blobService.DeleteBlobFromContainer(containerName, fileName);

                _imageServ.ThrowForbiddenImageException(file.ID, imageAnalysis);
            }
        }
    }
}
