using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
using Quantum.Integration.Internal.Models;
using Quantum.Integration.Internal.Services.Contracts;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Quantum.Integration.Internal.Services
{
	public class WebServerService : IWebServerService
	{
		private IConfiguration _config;
		private ILogger<WebServerService> _logger;
		private IHttpClientFactory _clientFactory;

		public WebServerService(
			IConfiguration config,
			ILogger<WebServerService> logger,
			IHttpClientFactory clientFactory
		)
		{
			_config = config;
			_logger = logger;
			_clientFactory = clientFactory;
		}

		public void SaveImageOnWebServer(List<SaveImageFileModel> saveImageFile)
		{
			var client = _clientFactory.CreateClient("save-image-file");
			var domain = client.BaseAddress.OriginalString;
			var path = _config["FrontApp:SaveImageFile"];
			var url = $"{domain}{path}";
			var jsonInString = JsonSerializer.Serialize(saveImageFile);
				//JsonConvert.SerializeObject(saveImageFile);
			HttpContent content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
			client.PostAsync(url, content);
		}
	}
}
