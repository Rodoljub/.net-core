using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quantum.Utility.Services.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Utility.Services
{
    public class UtilityService : IUtilityService
    {

        private ILogger<UtilityService> _logger;
        private UserManager<IdentityUser> _userMgr;
        private IConfiguration _config;

        public UtilityService(
            ILogger<UtilityService> logger,
            UserManager<IdentityUser> userMgr,
            IConfiguration config)
        {
            _logger = logger;
            _userMgr = userMgr;
            _config = config;
        }

        public async Task<IdentityUser> GetAuthUser(IIdentity userIdentity)
        {
            var identity = userIdentity as ClaimsIdentity;

            var userSubClaim = identity.Claims
                .FirstOrDefault(c => c.Type == "sub");

            if (userSubClaim != null)
            {
                var user = await _userMgr.FindByIdAsync(userSubClaim.Value);

                if (user != null)
                {
                    return user;
                }
            }

            //var userEmailClaim = identity.Claims
            //	.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            //if (userEmailClaim != null)
            //         {
            //             var user = await _userMgr.FindByEmailAsync(userEmailClaim.Value);

            //             if (user != null)
            //             {
            //                 return user;
            //             }
            //         }

            return null;
        }

        public string EncodeUrl(string url)
        {
            string[] splitUrl = url.Split(new[] { '=' }, 2);

            var encodeReturnUrl = WebUtility.UrlEncode(splitUrl[1]);

            var returnUrl = $"{splitUrl[0]}={encodeReturnUrl}";
            return returnUrl;
        }

        public string GetImagePath(string fileId, string fileExtansion)
        {
            var imagesRootUri = $"{_config["Application:AzureBlob:AbsoluteUri"]}{_config["Application:AzureBlob:ContainerImagesName"]}";

            return $"{imagesRootUri}/{fileId}.{fileExtansion}";
        }

        public DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public string TimeAgo(DateTime dateTime)
        {

            //DateTime dateTime = DateTime.UtcNow;
            string result = string.Empty;
            var timeSpan = DateTime.UtcNow.Subtract(dateTime);

            switch (timeSpan)
            {
                case var exp when timeSpan <= TimeSpan.FromSeconds(59):
                    result = timeSpan.Seconds < 1 ?
                        $"{_config["Application:TimeAgo:Now"]}" :
                        $"{timeSpan.Seconds}{_config["Application:TimeAgo:SecondsAgo"]}";
                    break;

                case var exp when timeSpan > TimeSpan.FromSeconds(59) && timeSpan <= TimeSpan.FromMinutes(60):
                    result = timeSpan.Minutes > 1 ?
                    $"{timeSpan.Minutes}{_config["Application:TimeAgo:MinutesAgo"]}" :
                    //String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    $"{timeSpan.Minutes}{_config["Application:TimeAgo:MinutesAgo"]}";
                    break;

                case var exp when timeSpan > TimeSpan.FromMinutes(60) && timeSpan <= TimeSpan.FromHours(24):
                    result = timeSpan.Hours > 1 ?
                    $"{timeSpan.Hours}{_config["Application:TimeAgo:HourseAgo"]}" :
                    $"{timeSpan.Hours}{_config["Application:TimeAgo:HourseAgo"]}";
                    break;

                case var exp when timeSpan > TimeSpan.FromHours(24) && timeSpan <= TimeSpan.FromDays(30):
                    result = timeSpan.Days > 1 ?
                    $"{timeSpan.Days}{_config["Application:TimeAgo:DaysAgo"]}" :
                    $"{timeSpan.Days}{_config["Application:TimeAgo:DaysAgo"]}";
                    break;

                case var exp when timeSpan > TimeSpan.FromDays(30) && timeSpan <= TimeSpan.FromDays(365):
                    result = timeSpan.Days > 30 ?
                    $"{365 / timeSpan.Days} {_config["Application:TimeAgo:MonthsAgo"]}" :
                    $"{timeSpan.Days} {_config["Application:TimeAgo:MonthAgo"]}";
                    break;

                case var exp when timeSpan > TimeSpan.FromDays(365):
                    result = timeSpan.Days > 365 ?
                    $"{timeSpan.Days / 365}{_config["Application:TimeAgo:YearsAgo"]}" :
                    $"{timeSpan.Days / 365}{_config["Application:TimeAgo:YearAgo"]}";
                    break;
            }

            return result;
        }

        public Image<Rgba32> ResizeImage(Image<Rgba32> original, int width, int height)
        {
            original.Mutate(x => x.Resize(width, height));
            return original;
        }

        public Image<Rgba32> ResizeImage(Image<Rgba32> original, int targetWidth)
        {
            int destWidth = targetWidth;
            int destHeight = (int)((double)original.Height / original.Width * targetWidth);

            original.Mutate(x => x.Resize(destWidth, destHeight));

            return original;
        }


        public bool ValidateImage(byte[] file, out Image<Rgba32> image)
        {
            bool output = false;
            image = null;

            try
            {
                using (var stream = new MemoryStream(file))
                {
                    image = Image.Load<Rgba32>(stream);
                    output = true;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace, ex.Message);
                throw;
            }

            return output;
        }

        public byte[] StreamToByteArray(Stream stream, int length)
        {
            byte[] fileContent = null;

            using (var binaryReader = new BinaryReader(stream))
            {
                fileContent = binaryReader.ReadBytes((int)length);
            }

            return fileContent;
        }

        public Stream ImageToStream(Image image, IImageEncoder format)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        public byte[] ImageToByteArray(Image<Rgba32> image, IImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        public string ByteArrayImageToBase64Image(byte[] image, string imageExtension, bool reduced, int width)
        {
            using (var memoryStream = new MemoryStream(image))
            {
                var imageSharpImage = Image.Load<Rgba32>(memoryStream);

                if (reduced)
                {
                    var resizedImageSharpImage = ResizeImage(imageSharpImage, width);
                    memoryStream.Position = 0;
                    resizedImageSharpImage.Save(memoryStream, GetImageFormat(imageExtension));
                }

                memoryStream.Position = 0;
                var base64ImageRepresentation = Convert.ToBase64String(memoryStream.ToArray());
                var base64 = $"data:image/{imageExtension};base64,{base64ImageRepresentation}";
                return base64;
            }
        }

        public IImageEncoder GetImageFormat(string imageExtension)
        {
            switch (imageExtension.ToLowerInvariant())
            {
                case "jpeg":
                case "jpg":
                    return new JpegEncoder();
                case "png":
                    return new PngEncoder();
                case "gif":
                    return new GifEncoder();
                default:
                    throw new ArgumentException("Invalid image extension");
            }
        }


        public string GenerateFileHash(Stream fs)
        {
            using (var hashAlg = SHA256.Create())
            {
                byte[] hashBytes = hashAlg.ComputeHash(fs);
                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }

        public Image UpdateImageOrientation(Image<Rgba32> image)
        {
            var metadata = image?.Metadata;

            if (metadata != null)
            {
                try
                {
                    var orientationValue = metadata.ExifProfile.Values.FirstOrDefault(v => v.Tag == ExifTag.Orientation);

                    if (((ushort)orientationValue.Tag) != 1)
                    {
                        image.Mutate(x => x.AutoOrient());
                    }

                    return image;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.StackTrace, ex.Message);
                    throw;
                }
            }

            return image;
        }

        private (RotateMode, FlipMode) GetOrientationToRotateMode(int orientationValue)
        {
            var rotateMode = RotateMode.None;
            var flipMode = FlipMode.None;

            switch (orientationValue)
            {
                case 1:
                    rotateMode = RotateMode.None;
                    break;
                case 2:
                    flipMode = FlipMode.Horizontal;
                    break;
                case 3:
                    rotateMode = RotateMode.Rotate180;
                    break;
                case 4:
                    flipMode = FlipMode.Horizontal;
                    rotateMode = RotateMode.Rotate180;
                    break;
                case 5:
                    flipMode = FlipMode.Vertical;
                    rotateMode = RotateMode.Rotate90;
                    break;
                case 6:
                    rotateMode = RotateMode.Rotate90;
                    break;
                case 7:
                    flipMode = FlipMode.Horizontal;
                    rotateMode = RotateMode.Rotate270;
                    break;
                case 8:
                    rotateMode = RotateMode.Rotate270;
                    break;
                default:
                    rotateMode = RotateMode.None;
                    break;
            }

            return (rotateMode, flipMode);
        }

        private Image<Rgba32> RotateImage(Image<Rgba32> image, float angle)
        {
            var rotateResampler = new BicubicResampler();
            var rotatedImage = image.Clone();
            rotatedImage.Mutate(x => x.Rotate(angle, rotateResampler));
            return rotatedImage;
        }

    }
}
