using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using File = Quantum.Data.Entities.File;

namespace Quantum.Core.Services.Contracts
{
    public interface IImageService
    {

        void ThrowForbiddenImageException(string imageId, ImageAnalysis imageAnalysis);

        void SaveImagesOnWebServer(byte[] imageContent, File file, string fileExstension);

        string[] GetBase64ImageArrayFromBase64StringRaw(string base64StringRaw, string userId);

        Task<string> GetBase64ImageByFileId(string fileId, bool reduced, int width);

        void ValidateImageFromByteArray(byte[] imageByteArray, string userId);
    }
}
