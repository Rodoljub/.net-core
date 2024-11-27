using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Quantum.Core.Services.ImageSharp.Contracts;
using Quantum.Utility.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services.ImageSharp
{
    public class ImageSharpService : IImageSharpService
    {
        private IConfiguration _config;
        public ImageSharpService(
                 IConfiguration config
        )
        {
            _config = config;
        }

        public async Task<Stream> ConvertImageToJpeg(Stream stream)
        {
            stream.Position = 0;

            var newStream = new System.IO.MemoryStream();
            await stream.CopyToAsync(newStream);
            newStream.Position = 0;

            using (var image = Image.Load(newStream))
            {
                var maxWidth = _config.GetAsInteger($"Application:AzureBlob:ImageWidth:1080", 1080);
                var imageWidth = image.Width;
                if (imageWidth > maxWidth)
                {
                    image.Mutate(c => c.Resize(maxWidth, 0));
                }

                var quality = _config.GetAsInteger($"Application:AzureBlob:ImageCompresionQuality:50", 50);
                var encoder = new JpegEncoder()
                {
                    Quality = quality
                };

                var memoryStream = new System.IO.MemoryStream();
                memoryStream.Position = 0;
                image.Save(memoryStream, encoder);
                memoryStream.ToArray();

                return memoryStream;
            }
        }

        public Task<Stream> ConvertImageToWebP(Stream image)
        {
            throw new NotImplementedException();
        }

        public async Task TestQuality(IFormFile formFile)
        {
            var stream = formFile.OpenReadStream();

            var newStram = await ConvertImageToJpeg(stream);

            var path = Path.Combine("C:\\Users\\ZERO-USER\\Desktop\\desktop\\images\\A", formFile.FileName);
            using (var fileStream = File.Create(path))
            {
                newStram.Seek(0, SeekOrigin.Begin);
                newStram.CopyTo(fileStream);

                //fileStream.Write(newStram.)
            }

        }




        public Stream TransformImageIfNeeded(Stream imagestream)
        {
            imagestream.Position = 0;
            using (var image = Image.Load(imagestream))
            {
                image.Mutate(x => x.AutoOrient());
               
                return ImageToByteArray(image);
            }
        }

        private Stream ImageToByteArray(Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, new JpegEncoder());
            stream.Position = 0;
            return stream;
        }


    }
}
