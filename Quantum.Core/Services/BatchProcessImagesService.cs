using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quantum.Core.Services.Contracts;
using Quantum.Core.Services.ImageSharp.Contracts;
using Quantum.Data.Repositories.Contracts;
using Quantum.Integration.External.Services.Contracts;
using Quantum.Integration.Services.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using Quantum.Utility.Services.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
    public class BatchProcessImagesService : IHostedService
    {
        private Thread _loopThread;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private ILogger<BatchProcessImagesService> _logger;
        private IItemRepository _itemRepo;
        private IFileRepository _fileRepo;
        private IItemsService _itemServ;
        private IConfiguration _config;
        private IBlobsStorageService _blobService;
        private IImageService _imageServ;
        private IComputerVisionService _computerVisionServ;
        private IUtilityService _utilServ;
        private IUserProfileService _userProfileServ;
        private IImageSharpService _imageSharpServ;


        public BatchProcessImagesService(
            ILogger<BatchProcessImagesService> logger,
            IConfiguration config,
            IServiceScopeFactory serviceFactory
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var scope = serviceFactory.CreateScope();
            _fileRepo = scope.ServiceProvider.GetService<IFileRepository>();
            _itemRepo = scope.ServiceProvider.GetService<IItemRepository>();
            _itemServ = scope.ServiceProvider.GetService<IItemsService>();
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _blobService = scope.ServiceProvider.GetService<IBlobsStorageService>(); 
            _imageServ = scope.ServiceProvider.GetService<IImageService>(); 
            _computerVisionServ =  scope.ServiceProvider.GetService<IComputerVisionService>();
            _utilServ =  scope.ServiceProvider.GetService<IUtilityService>();
            _userProfileServ = scope.ServiceProvider.GetService<IUserProfileService>();
            _imageSharpServ = scope.ServiceProvider.GetService<IImageSharpService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            

            _loopThread = new Thread(async () =>
            {
                _logger.LogInformation("Starting _loopThread....");

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var threadSleep = _config.GetAsInteger($"Application:ComputerVision:AnalyzeImage:AnaiyzeImageThreadSleepMs", 65000);
                    Thread.Sleep(threadSleep);

                    await AnalyzeFile();
                }
            });

            _logger.LogInformation("Starting BatchProcessImagesService....");
            _loopThread.Start();
            return Task.CompletedTask;
        }

        private async Task AnalyzeFile()
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
                            var format = _utilServ.GetImageFormat(file.Extension);
                            int width640 = _config.GetValue<int>($"Application:ImageSize:640", 640);
                            Image resizedImageForAnalyze = _utilServ.ResizeImage(image, width640);
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stoping _loopThread....");
            _cancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }

    }
}
