using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quantum.Integration.External.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Integration.External.Services
{
    public class BlobsStorageService : IBlobsStorageService
    {
        private IConfiguration _config;
        private ILogger<BlobsStorageService> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public BlobsStorageService(
            IConfiguration config,
            ILogger<BlobsStorageService> logger,
            BlobServiceClient blobServiceClient
        )
        {
            _config = config ?? throw new ArgumentNullException(nameof(config)); ;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        }
        public async Task<BlobContainerClient> CreateBlobContainerAsync(string containerName, bool createPublic = false)
        {
            // Create the container and return a container client object
            if (createPublic)
            {
                BlobContainerClient containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);
                return containerClient;
            }
            else
            {
                BlobContainerClient containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                return containerClient;
            }
        }

        public async Task DeleteBlobContainerAsync(string containerName)
        {
            BlobContainerClient container = GetContainerClient(containerName);

            try
            {
                // Delete the specified container and handle the exception.
                await container.DeleteAsync();
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, ex.StackTrace, ex.Message);
                throw;
            }
        }

        public BlobContainerClient GetContainerClient(string blobContainerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            return containerClient;
        }

        public async Task<bool> UploadBlobImageToContainer(string blobContainerName, string fileName, Stream imageStream, string contentType)
        {
            var successUploading = false;
            BlobClient blobClient = GetBlobClient(blobContainerName, fileName);
            try
            {
                var response = await blobClient.UploadAsync(imageStream);
                blobClient = await SetBlobPropertiesAsync(blobClient, contentType);
                successUploading = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace, ex.Message);
                throw;
            }
            
            return successUploading;
        }

        public Task<Response<bool>> DeleteBlobFromContainer(string blobContainerName, string fileName)
        {
            BlobClient blobClient = GetBlobClient(blobContainerName, fileName);

            return blobClient.DeleteIfExistsAsync();
        }

        public async Task UploadWebP(Stream stream, string fileId)
        {
            var blobContainerName = _config["Application:AzureBlob:ContainerImagesName"];
            var webPExt = _config["Application:AzureBlob:WebPExtension"];
            var fileName = $"{fileId}{webPExt}";
            BlobClient blobClient = GetBlobClient(blobContainerName, fileName);
            try
            {
                var response = await blobClient.UploadAsync(stream);
                var contentType = _config["Application:AzureBlob:WebPContentType"];
                blobClient = await SetBlobPropertiesAsync(blobClient, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace, ex.Message);
                throw;
            }
        }

        public async Task UploadJpeg(Stream jpegStream, string fileId)
        {
            var blobContainerName = _config["Application:AzureBlob:ContainerImagesName"];
            var jpegExt = _config["Application:AzureBlob:JpegExtension"];
            var fileName = $"{fileId}{jpegExt}";
            BlobClient blobClient = GetBlobClient(blobContainerName, fileName);
            try
            {

                var response = await blobClient.UploadAsync(jpegStream);
                var contentType = _config["Application:AzureBlob:JpegContentType"];
                blobClient = await SetBlobPropertiesAsync(blobClient, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace, ex.Message);
                throw;
            }
        }

        public async Task GetBlobsFlatListening(string blobContainerName, int? segmentSize)
        {
            BlobContainerClient blobContainerClient = GetContainerClient(blobContainerName);

            await ListBlobsFlatListing(blobContainerClient, segmentSize);
        }

        private BlobClient GetBlobClient(string blobContainerName, string fileName)
        {
            BlobContainerClient containerClient = GetContainerClient(blobContainerName);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            return blobClient;
        }

        private async Task ListBlobsFlatListing(BlobContainerClient blobContainerClient, int? segmentSize)
        {
            try
            {
                // Call the listing operation and return pages of the specified size.
                var resultSegment = blobContainerClient.GetBlobsAsync()
                    .AsPages(default, segmentSize);

                // Enumerate the blobs returned for each page.
                await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                {
                    foreach (BlobItem blobItem in blobPage.Values)
                    {
                        Console.WriteLine("Blob name: {0}", blobItem.Name);
                    }

                    Console.WriteLine();
                }
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, ex.StackTrace, ex.Message);
                throw;
            }
        }

        public async Task UpdateBlobContentType(string blobContainerName, string fileName, string fileContentType)
        {
            var blobClient = GetBlobClient(blobContainerName, fileName);

            blobClient = await SetBlobPropertiesAsync(blobClient, fileContentType);

        }

        public async Task<BlobClient> SetBlobPropertiesAsync(BlobClient blob, string contentType)
        {
            try
            {
                // Get the existing properties
                BlobProperties properties = await blob.GetPropertiesAsync();

                BlobHttpHeaders headers = new BlobHttpHeaders
                {
                    // Set the MIME ContentType every time the properties 
                    // are updated or the field will be cleared
                    ContentType = contentType,
                    ContentLanguage = "en-us",

                    // Populate remaining headers with 
                    // the pre-existing properties
                    CacheControl = properties.CacheControl,
                    ContentDisposition = properties.ContentDisposition,
                    ContentEncoding = properties.ContentEncoding,
                    ContentHash = properties.ContentHash
                };

                // Set the blob's properties.
                await blob.SetHttpHeadersAsync(headers);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, ex.StackTrace, ex.Message);
                throw;
            }

            return blob;
        }

        public async Task<Stream> GetBlobStream(string blobContainerName, string fileName)
        {
            BlobClient blobClient = GetBlobClient(blobContainerName, fileName);

            var blobStream = await blobClient.ExistsAsync();

            if (blobStream.Value)
            {
                return await blobClient.OpenReadAsync();
            }
            else
            {
                return null;
            }

        }

        public async Task DeleteWebPJpegImages(string fileId)
        {
            var blobContainerName = _config["Application:AzureBlob:ContainerImagesName"];
            var webPExt = _config["Application:AzureBlob:WebPExtension"];
            //var webPFileName = $"{fileId}{webPExt}";
            var jpegExt = _config["Application:AzureBlob:JpegExtension"];
            var jpegFileName = $"{fileId}{jpegExt}";

            //await DeleteBlobFromContainer(blobContainerName, webPFileName);
            await DeleteBlobFromContainer(blobContainerName, jpegFileName);
        }
    }
}
