using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quantum.Core.Services.Auth.Contracts;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Auth
{
    public class DocumentService : IDocumentService
    {
        private ILogger<DocumentService> _logger;
        private IFileRepository _fileRepo;


        public DocumentService(
            ILogger<DocumentService> logger,
            IServiceScopeFactory serviceFactory
        )
        {
            _logger = logger;

            var scope = serviceFactory.CreateScope();
            _fileRepo = scope.ServiceProvider.GetService<IFileRepository>();
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
