using Microsoft.Extensions.Logging;
using Quantum.AuthorizationServer.Services.Contracts;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services
{
    public class DocumentService : IDocumentService
    {
        private ILogger<DocumentService> _logger;
        private IFileRepository _fileRepo;

        public DocumentService(
            ILogger<DocumentService> logger,
            IFileRepository fileRepo
        )
        {
            _logger = logger;
            _fileRepo = fileRepo;
        }

        public async Task<string> GetFileContentAsString(string fileName)
        {
            _logger.LogInformation($"Start Query File '{fileName}'");

            var templateFile = await _fileRepo.GetFileByName(fileName);

            _logger.LogInformation($"End Query File '{fileName}'");

            if (templateFile == null || templateFile.file == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.InternalServerError,
                    string.Empty, Errors.GeneralError, null, $"File name '{fileName}' cannot be found!");
            }

            var templateHtml = Encoding.UTF8.GetString(templateFile.file);

            return templateHtml;
        }
    }
}
