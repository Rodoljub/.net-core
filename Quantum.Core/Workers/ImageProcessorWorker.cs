using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quantum.Core.Services.Contracts;
using Quantum.Core.Services.ImageSharp.Contracts;
using Quantum.Data.Repositories.Contracts;
using Quantum.Integration.External.Services.Contracts;
using Quantum.Integration.Services.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Services.Contracts;
using Quartz;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Workers
{
    public class ImageProcessorWorker : IJob
    {
        private ILogger<ImageProcessorWorker> _logger;

        private IFileRepository _fileRepo;
        private IItemsService _itemServ;
        private IConfiguration _config;
        private IBlobsStorageService _blobService;
        private IComputerVisionService _computerVisionServ;
        private IUtilityService _utilServ;
        private IUserProfileService _userProfileServ;
        private IImageSharpService _imageSharpServ;

        public ImageProcessorWorker(
            ILogger<ImageProcessorWorker> logger,
            IFileRepository fileRepo,
            IItemsService itemServ,
            IConfiguration config,
            IBlobsStorageService blobService,
            IComputerVisionService computerVisionServ,
            IUtilityService utilServ,
        IUserProfileService userProfileServ,
        IImageSharpService imageSharpServ
             )
        {
            ArgumentNullException.ThrowIfNull(logger);
            _logger = logger;// !!;

            ArgumentNullException.ThrowIfNull(fileRepo);
            _fileRepo = fileRepo;// !!;

            ArgumentNullException.ThrowIfNull(itemServ);
            _itemServ = itemServ;

            ArgumentNullException.ThrowIfNull(config);
            _config = config;

            ArgumentNullException.ThrowIfNull(blobService);
            _blobService = blobService;

            ArgumentNullException.ThrowIfNull(computerVisionServ);
            _computerVisionServ = computerVisionServ;

            ArgumentNullException.ThrowIfNull(utilServ);
            _utilServ = utilServ;

            ArgumentNullException.ThrowIfNull(userProfileServ);
            _userProfileServ = userProfileServ;

            ArgumentNullException.ThrowIfNull(utilServ);
            _imageSharpServ = imageSharpServ;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Starting Execution @ {DateTime.Now}");

            int take = _config.GetValue<int>($"ImageProcessorWorker:Take", 7);

            List<Data.Entities.File> filesForAnalysis = await _fileRepo.GetFilesForAnalysis(take);

            _logger.LogInformation($"Total count of files:{filesForAnalysis.Count}");

            if (filesForAnalysis.Any())
            {
                string containerName = _config["Application:AzureBlob:ContainerImagesForAnalysisName"];

                foreach (var file in filesForAnalysis)
                {
                    _logger.LogInformation($"Processing file:{file.ID}");

                    try
                    {
                        string fileName = $"{file.ID}.{file.Extension}";
                        Stream stream = await _blobService.GetBlobStream(containerName, fileName);

                        if (stream != null)
                        {
                            var image = Image.Load<Rgba32>(stream);

                            //_logger.LogError($"Original image: {image.ToBase64String(image.Metadata.DecodedImageFormat)}");

                            var format = _utilServ.GetImageFormat(file.Extension);
                            int width640 = _config.GetValue<int>($"Application:ImageSize:640", 640);

                            Image resizedImageForAnalyze = _utilServ.ResizeImage(image, width640);

                            //_logger.LogError($"ResizedImageForAnalyze: {resizedImageForAnalyze.ToBase64String(resizedImageForAnalyze.Metadata.DecodedImageFormat)}");

                            Stream resizedImageForAnalyzeStream = _utilServ.ImageToStream(resizedImageForAnalyze, format);

                            ImageAnalysis imageAnalysis = await _computerVisionServ.AnalyzeImageWithCVClient(resizedImageForAnalyzeStream);

                            switch (file.FileType.Name)
                            {
                                case FileTypes.Images.ProjectFile:
                                    await _itemServ.UpdateAnalysedItem(file, imageAnalysis);
                                    break;

                                case FileTypes.Images.ProfileImage:
                                    await _userProfileServ.UpdateAnalysedProfileImage(file, imageAnalysis);
                                    break;

                                default:
                                    break;
                            }

                            stream.Position = 0;
                            Stream jpegStream = await _imageSharpServ.ConvertImageToJpeg(stream);
                            jpegStream.Position = 0;
                            await _blobService.UploadJpeg(jpegStream, file.ID);

                        }
                        else
                        {
                            await _fileRepo.Delete(file.ID, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message, ex.StackTrace);
                    }
                }
            }
        }
    }
}
