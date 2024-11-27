using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Core.Services.ImageSharp.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
using Quantum.Data.Repositories.Contracts;
using Quantum.Integration.External.Services.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using Quantum.Utility.Infrastructure.Exceptions;
using Quantum.Utility.Services.Contracts;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using File = Quantum.Data.Entities.File;

namespace Quantum.Core.Services
{
    public class ItemsService : IItemsService
    {
        private ILogger<ItemsService> _logger;
        private readonly IConfiguration _config;
        private UserManager<IdentityUser> _userMgr;
        private IUserManagerService _userMgrServ;
        private IMapper _mapper;

        private IItemRepository _itemRepo;
        private IFileRepository _fileRepo;
        private IFileDetailsRepository _fileDetailsRepo;
        private IFileTypeRepository _fileTypeRepo;
        private IImageService _imageServ;
        private IBlobsStorageService _blobService;
        private IUtilityService _utilServ;
        private IFileService _fileServ;
        private ITagService _tagServ;
        private IImageSharpService _imgSharpServ;
        private IUserProfileRepository _userProfileRepo;
        private INotificationService _notificationServ;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IServiceProvider Services { get; }
        public IBackgroundTaskQueue Queue { get; }

        public ItemsService(
            ILogger<ItemsService> logger,
            IConfiguration config,
            UserManager<IdentityUser> userMgr,
            IUserManagerService userMgrServ,
            IMapper mapper,
            IItemRepository itemRepo,
            IFileRepository fileRepo,
            IFileDetailsRepository fileDetailsRepo,
            IFileTypeRepository fileTypeRepo,
            IImageService imageServ,
            IBlobsStorageService blobService,
            IUtilityService utilServ,
            IFileService fileServ,
            ITagService tagServ,
            IImageSharpService imgSharpServ,
            IUserProfileRepository userProfileRepo,
            INotificationService notificationServ,
            IServiceScopeFactory serviceScopeFactory,
            IServiceProvider services,
            IBackgroundTaskQueue queue
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
            _userMgrServ = userMgrServ ?? throw new ArgumentNullException(nameof(userMgrServ));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
            _fileRepo = fileRepo ?? throw new ArgumentNullException(nameof(fileRepo));
            _fileDetailsRepo = fileDetailsRepo ?? throw new ArgumentNullException(nameof(fileDetailsRepo));
            _fileTypeRepo = fileTypeRepo ?? throw new ArgumentNullException(nameof(fileTypeRepo));
            _imageServ = imageServ ?? throw new ArgumentNullException(nameof(imageServ));
            _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
            _utilServ = utilServ ?? throw new ArgumentNullException(nameof(utilServ));
            _fileServ = fileServ ?? throw new ArgumentNullException(nameof(fileServ));
            _tagServ = tagServ ?? throw new ArgumentNullException(nameof(tagServ));
            _imgSharpServ = imgSharpServ;
            _userProfileRepo = userProfileRepo ?? throw new ArgumentNullException(nameof(userProfileRepo));
            _notificationServ = notificationServ ?? throw new ArgumentNullException(nameof(notificationServ));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        public async Task<AnalyzingImageViewModel> CreateItemForAnalysis(ItemCreateModel model, IIdentity identity)
        {
            _fileServ.ValidateItemFile(model.File);

            IdentityUser user = await _userMgrServ.GetAuthUser(identity);

            var newStream = new MemoryStream();
            await model.File.CopyToAsync(newStream);

            _imageServ.ValidateImageFromByteArray(newStream.ToArray(), user.Id);

            File file = await MapAndInsertItemFile(model, user);

            var image = await SaveImageOnBlobStorage(model.File, file);

            if (image != null)
            {
                await _fileRepo.Save();
                await MapAndInsertItem(model, user, file.ID);
                return image;
            }
            else
            {
                throw new GeneralErrorException(HttpStatusCode.BadRequest, Errors.GeneralError);
            };
        }
        private async Task<AnalyzingImageViewModel> SaveImageOnBlobStorage(IFormFile formFile, File file)
        {
            var containerName = _config["Application:AzureBlob:ContainerImagesForAnalysisName"];
            var fileName = $"{file.ID}.{file.Extension}";

            var imageStream = _imgSharpServ.TransformImageIfNeeded(formFile.OpenReadStream());
            //image = _utilServ.UpdateImageOrientation(image);
            //Stream imageStream = image.ToStream();

            //Stream imageStream = ImageOrientation;
            //imageStream.Position = 0;
            var contentType = formFile.ContentType;
            var isSuccessUpload = await _blobService.UploadBlobImageToContainer(containerName, fileName, imageStream, contentType);

            if (isSuccessUpload)
            {
                imageStream.Position = 0;
                var imageBase64 = GetAnalyzingImageViewModel(file, imageStream);
                return imageBase64;
            }

            return null;

        }

        public async Task UpdateAnalysedItem(File file, ImageAnalysis imageAnalysis)
        {
            var item = await _itemRepo.GetItemByFileId(file.ID);

            //_logger.LogError

            if (item != null)
            {
                IdentityUser user = await _userMgr.FindByIdAsync(item.CreatedById);

                await DeleteItemAndThrowExceptionIfImageForbidden(item, item.FileID, imageAnalysis, user);

                await _tagServ.CreateItemTags(imageAnalysis, item, user);

                await MapAndInsertFileDetails(item, imageAnalysis, user);

                await _itemRepo.Undelete(item, user);

                QueueSetNotificationAndEvent(item, user);
                UpdateUserProfileImagesAggregation(item.CreatedById, user);
            }
            else
            {
                await _fileRepo.Remove(file.ID);
            }
        }

        private async Task MapAndInsertFileDetails(Item item, ImageAnalysis analyzedImage, IdentityUser user)
        {
            var fileDetails = _mapper.Map<ImageAnalysis, FileDetails>(analyzedImage);

            fileDetails.ImageAnalysis = JsonConvert.SerializeObject(analyzedImage);

            var fileDetailsId = await _fileDetailsRepo.InsertFileDetails(fileDetails, user);

            item.File.FileDetailsId = fileDetailsId;

            await _fileRepo.Update(item.File, user);

        }

        public async Task<string> UpdateItem(ItemUpdateModel model, IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);
            var userProfile = await _userProfileRepo.GetByUserId(user.Id);
            var item = await _itemRepo.GetItemById(model.ItemId);

            if (item.UserProfile.ID == userProfile.ID)
            {
                item.Description = model.Description ?? string.Empty;

                await _itemRepo.UpdateItem(item, user);

                await _tagServ.UpdateDisplayTags(item, model, user);

                return item.ID;
            }

            return default;
        }

        public async Task<string> DeleteItem(string itemId, IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);
            var userProfile = await _userProfileRepo.GetByUserId(user.Id);
            var item = await _itemRepo.GetItemById(itemId);

            if (item.UserProfile.ID == userProfile.ID)
            {
                var containerName = _config["Application:AzureBlob:ContainerImagesForAnalysisName"];
                var fileName = _utilServ.GetImagePath(item.FileID, item.File.Extension);

                await _itemRepo.Delete(item, user);

                await _notificationServ.DeleteNotification(itemId, identity);

                await _blobService.DeleteBlobFromContainer(containerName, fileName);

                await _blobService.DeleteWebPJpegImages(item.FileID);

                UpdateUserProfileImagesAggregation(item.CreatedById, user);

                return itemId;
            }

            return default;
        }

        public async Task<ItemViewModel> GetItemView(string itemId, IIdentity identity)
        {
            //Item item = await _itemRepo.GetItemById(itemId);

            var userId = string.Empty;
            var user = await _userMgrServ.GetAuthUser(identity);

            if (user != null)
            {
                userId = user.Id;
            }

            //var tags = item.ItemTags.Select(it => it.Tag);

            //var width = _config.GetAsInteger($"Application:ImageSize:640", 640);

            ////var viewItem = await 

            //var viewItem = await _mappingItemsServ.MapItemViewModelFromItem(item, 1, tags, false, userId, width);

            var viewItem = await _itemRepo.GetItemViewModel(itemId, userId);

            return viewItem;
        }


        private async Task DeleteItemAndThrowExceptionIfImageForbidden(Item item, string fileId, ImageAnalysis imageAnalysis, IdentityUser user)
        {
            if (imageAnalysis.Adult.IsAdultContent || imageAnalysis.Adult.IsRacyContent)
            {
                await _fileRepo.Delete(item.FileID, user);
                await _itemRepo.Delete(item, user);

                var containerName = _config["Application:AzureBlob:ContainerImagesForAnalysisName"];
                var fileName = _utilServ.GetImagePath(item.FileID, item.File.Extension);
                await _blobService.DeleteBlobFromContainer(containerName, fileName);

                _imageServ.ThrowForbiddenImageException(fileId, imageAnalysis);
            }
        }


        private void QueueSetNotificationAndEvent(Item item, IdentityUser user)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var scopedNotificationService = scopedServices.GetRequiredService<INotificationService>();

                    await scopedNotificationService.ProjectFileReady(item, user);
                }
            });
        }

        private void UpdateUserProfileImagesAggregation(string userId, IdentityUser user)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var scopedItemsListRepo = scopedServices.GetRequiredService<IItemsListRepository>();

                    var scopedUserProfileRepo = scopedServices.GetRequiredService<IUserProfileRepository>();

                    var userImagesCount = await scopedItemsListRepo.CountUserItemsByUserId(userId);

                    var userProfile = await scopedUserProfileRepo.GetByUserIdNoTrack(userId);

                    userProfile.UploadsCount = userImagesCount;

                    await scopedUserProfileRepo.Update(userProfile, user);
                }
            });
        }

        private async Task<File> MapAndInsertItemFile(ItemCreateModel model, IdentityUser user)
        {
            FileType fileType = await _fileTypeRepo.GetFileTypeByName(FileTypes.Images.ProjectFile);

            File file = new File
            {
                FileType = fileType,
                Extension = Path.GetExtension(model.File.FileName).TrimStart('.'),
                //Name = model.File.FileName
            };

            return await _fileRepo.InsertFile(file, user, false);
        }

        private async Task<Item> MapAndInsertItem(ItemCreateModel model, IdentityUser user, string fileId)
        {
            string userProfileId = await _userProfileRepo.GetUserProfileIdByUserId(user.Id);

            Item item = new Item()
            {
                Description = model.Description,
                FileID = fileId,
                UserProfileId = userProfileId
            };
            await _itemRepo.Insert(item, user);
            //await _itemRepo.Delete(item, user);

            return item;
        }

        public async Task<List<AnalyzingImageViewModel>> GetAnalyzingImages(string[] filesIds, IIdentity identity)
        {
            IdentityUser user = await _userMgrServ.GetAuthUser(identity);

            var analyzingImages = new List<AnalyzingImageViewModel>();

            var files = await _fileRepo.GetItemsFilesAnalyzingByUserId(user.Id);

            foreach (var file in files)
            {
                string containerName = _config["Application:AzureBlob:ContainerImagesForAnalysisName"];

                string fileName = $"{file.ID}.{file.Extension}";
                Stream stream = await _blobService.GetBlobStream(containerName, fileName);
                stream.Position = 0;
                var newStream = new System.IO.MemoryStream();
                stream.CopyTo(newStream);
                newStream.Position = 0;
                //var byteArray = newStream.ToArray();

                AnalyzingImageViewModel analyzingImage = GetAnalyzingImageViewModel(file, newStream);

                analyzingImages.Add(analyzingImage);
            }

            return analyzingImages;
        }

        private AnalyzingImageViewModel GetAnalyzingImageViewModel(File file, Stream imageStream)
        {
            //var newStream = new System.IO.MemoryStream();
            //stream.CopyTo(newStream);
            //newStream.Position = 0;
            int width80 = _config.GetAsInteger($"Application:ImageSize:80", 80);
            var byteArray = _utilServ.StreamToByteArray(imageStream, (int)imageStream.Length);
            var base64 = _utilServ.ByteArrayImageToBase64Image(byteArray, file.Extension, true, width80);



            var analyzingImage = new AnalyzingImageViewModel()
            {
                FileId = file.ID,
                FileExtension = file.Extension,
                Image = base64
            };
            return analyzingImage;
        }

        public void Aggragatiosns(string userId)
        {
            UpdateUserProfileImagesAggregation(userId, null);
        }
    }
}
