using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quantum.Integration.Services.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace Quantum.ExternalIntegration.Services
{
	public class ComputerVisionService : IComputerVisionService
	{
		private IConfiguration _config;
		private ILogger<ComputerVisionService> _logger;
		private readonly IComputerVisionClient _computerVisionClient;

		public ComputerVisionService(
			IConfiguration config,
			ILogger<ComputerVisionService> logger,
			IComputerVisionClient computerVisionClient
		)
		{
			_config = config;
			_logger = logger;
			_computerVisionClient = computerVisionClient;
		}

		public async Task<ImageAnalysis> AnalyzeImageWithCVClient(Stream imageStream)
        {

			List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>(){
				VisualFeatureTypes.Brands,
				VisualFeatureTypes.Categories,
				VisualFeatureTypes.Description,
				VisualFeatureTypes.Faces,
				VisualFeatureTypes.ImageType,
				VisualFeatureTypes.Tags,
				VisualFeatureTypes.Color,
				VisualFeatureTypes.Adult
			};

			List<Details?> details = new List<Details?>() { 
				Details.Celebrities,
				Details.Landmarks
			};

			string language = _config["Application:ComputerVision:AnalyzeImage:Language"];
			ImageAnalysis result = await _computerVisionClient
				.AnalyzeImageInStreamAsync(imageStream, features, details, language);
			
			
			return result;
		}

		public static ComputerVisionClient Authenticate(string endpoint, string key)
		{
			ComputerVisionClient client =
			  new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
			  { Endpoint = endpoint };
			return client;
		}
	}
}
