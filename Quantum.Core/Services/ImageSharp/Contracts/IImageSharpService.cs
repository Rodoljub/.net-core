using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services.ImageSharp.Contracts
{
    public interface IImageSharpService
    {
        Task<Stream> ConvertImageToWebP(Stream image);
        Task<Stream> ConvertImageToJpeg(Stream stream);
        Task TestQuality(IFormFile formFile);

        Stream TransformImageIfNeeded(Stream imagestream);
    }
}
