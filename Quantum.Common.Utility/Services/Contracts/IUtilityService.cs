using Microsoft.AspNetCore.Identity;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Utility.Services.Contracts
{
    public interface IUtilityService
    {
        Task<IdentityUser> GetAuthUser(IIdentity userIdentity);

        string EncodeUrl(string url);

        string GetImagePath(string fileId, string fileExtansion);
        DateTime ConvertFromUnixTimestamp(double timestamp);

        string TimeAgo(DateTime dateTime);

        Image<Rgba32> ResizeImage(Image<Rgba32> image, int width, int height);

        Image<Rgba32> ResizeImage(Image<Rgba32> original, int targetWidth);

        bool ValidateImage(byte[] file, out Image<Rgba32> image);

        byte[] StreamToByteArray(Stream stream, int length);

        byte[] ImageToByteArray(Image<Rgba32> image, IImageFormat format);

        string ByteArrayImageToBase64Image(byte[] image, string imageExstension, bool reduced, int width);

        string GenerateFileHash(Stream fs);

        Image UpdateImageOrientation(Image<Rgba32> image);

        IImageEncoder GetImageFormat(string imageExtension);

        Stream ImageToStream(Image image, IImageEncoder format);
    }
}
