using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Repositories.Contracts;
using Quantum.Integration.Internal.Models;
using Quantum.Integration.Internal.Services.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using Quantum.Utility.Infrastructure.Exceptions;
using Quantum.Utility.Services.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using File = Quantum.Data.Entities.File;

namespace Quantum.Core.Services
{
    public class ImageService : IImageService
    {
        private IConfiguration _config;
        private IUtilityService _utilServ;

        private IFileService _fileServ;
        private IWebServerService _webServerServ;
        private IFileRepository _fileRepo;
        private IMappingImageService _mappingImageServ;


        public ImageService(
            IConfiguration config,
            IUtilityService utilServ,
            IFileService fileServ,
            IWebServerService webServerServ,
            IFileRepository fileRepo,
            IMappingImageService mappingImageServ
        )
        {
            _config = config;
            _utilServ = utilServ;
            _fileServ = fileServ;
            _webServerServ = webServerServ;
            _fileRepo = fileRepo;
            _mappingImageServ = mappingImageServ;
        }

        public void ThrowForbiddenImageException(string imageId, ImageAnalysis imageAnalysis)
        {
            if (imageAnalysis.Adult.IsAdultContent)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Forbidden,
                   null,
                   Errors.ErrorAdultContentImage,
                   null,
                   $"Adult Content Image: '{imageId}'.");
            }

            if (imageAnalysis.Adult.IsRacyContent)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Forbidden,
                   null,
                   Errors.ErrorRacyContentImage,
                   null,
                   $"Racy Content Image: '{imageId}'.");
            }
        }

        public void SaveImagesOnWebServer(byte[] imageContent, File file, string fileExstension)
        {
            new Task(() =>
            {
                List<int> listImageWidth = new List<int>();
                listImageWidth.Add(_config.GetAsInteger($"Application:ImageSize:320", 320));

                if (file.FileType.Name == FileTypes.Images.ProjectFile)
                    listImageWidth.Add(_config.GetAsInteger($"Application:ImageSize:640", 640));

                List<SaveImageFileModel> listSaveImagesFiles = new List<SaveImageFileModel>();

                foreach (var width in listImageWidth)
                {
                    string base64Image = _utilServ.ByteArrayImageToBase64Image(imageContent, fileExstension, true, width);
                    var saveImageFile = _mappingImageServ.MapSaveImageFileModelFromFile(file, base64Image, width).GetAwaiter().GetResult();

                    listSaveImagesFiles.Add(saveImageFile);

                }

                _webServerServ.SaveImageOnWebServer(listSaveImagesFiles);
            }).Start();
        }

        public string[] GetBase64ImageArrayFromBase64StringRaw(string base64StringRaw, string userId)
        {
            if (string.IsNullOrWhiteSpace(base64StringRaw))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                 null,
                 Errors.ErrorImageFormat,
                 null,
                 $"User: '{userId}' uploaded invalid image format");
            }

            string[] base64ImageStringArray = base64StringRaw.Split(new string[] { ";base64," }, StringSplitOptions.None);

            Validatebase64ImageStringArray(userId, base64ImageStringArray);

            return base64ImageStringArray;
        }

        private void Validatebase64ImageStringArray(string userId, string[] base64ImageStringArray)
        {
            if (base64ImageStringArray.Length < 2)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                 null,
                 Errors.ErrorImageFormat,
                 null,
                 $"User: '{userId}' uploaded invalid image format");
            }

            var isImage = base64ImageStringArray[0].StartsWith("data:image/");

            if (!isImage)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                 null,
                 Errors.ErrorImageFormat,
                 null,
                 $"User: '{userId}' uploaded invalid image format");
            }
        }

        public void ValidateImageFromByteArray(byte[] imageByteArray, string userId)
        {
            Image<Rgba32> image = null;

            if (!_utilServ.ValidateImage(imageByteArray, out image))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                 null,
                 Errors.ErrorImageFormat,
                 null,
                 $"User: '{userId}' uploaded invalid image format");
            }
        }

        public async Task<string> GetBase64ImageByFileId(string fileId, bool reduced, int width)
        {
            var file = await _fileRepo.GetFileById(fileId);

            return _utilServ.ByteArrayImageToBase64Image(file.file, file.Extension, reduced: reduced, width: width);
        }

    }
}
