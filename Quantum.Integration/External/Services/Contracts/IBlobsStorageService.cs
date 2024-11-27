using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Integration.External.Services.Contracts
{
    public interface IBlobsStorageService
    {
        Task<BlobContainerClient> CreateBlobContainerAsync(string containerName, bool createPublic = false);

        Task DeleteBlobContainerAsync(string containerName);

        Task<bool> UploadBlobImageToContainer(string blobContainerName, string fileName, Stream imageStream, string contentType);

        Task<Response<bool>> DeleteBlobFromContainer(string blobContainerName, string fileName);

        BlobContainerClient GetContainerClient(string blobContainerName);

        Task<Stream> GetBlobStream(string blobContainerName, string fileName);

        Task GetBlobsFlatListening(string blobContainerName, int? segmentSize);

        Task UpdateBlobContentType(string blobContainerName, string fileName, string fileContentType);
     
        Task UploadWebP(Stream stream, string fileName);
     
        Task UploadJpeg(Stream jpegStream, string fileId);

        Task DeleteWebPJpegImages(string fileId);
    }
}
