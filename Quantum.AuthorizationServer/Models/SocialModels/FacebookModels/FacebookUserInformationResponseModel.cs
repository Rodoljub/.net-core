using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
    public class FacebookUserInformationResponseModel
    {
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("picture")]
		public Picture Picture { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }
	}

	public class Picture
	{
		[JsonProperty("data")]
		public PictureData PictureData { get; set; }
	}

	public class PictureData
	{
		[JsonProperty("is_silhouette")]
		public bool IsSilhouette { get; set; }

		[JsonProperty("height")]
		public bool Height { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("width")]
		public string Width { get; set; }
	}
}
