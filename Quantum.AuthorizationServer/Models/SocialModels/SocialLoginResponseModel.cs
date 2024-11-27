using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models.SocialModels
{
    public class SocialLoginResponseModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("expiration")]
        public DateTime Expiration { get; set; }

        public string ReturnUrl { get; set; }

        public bool UserExists { get; set; }
    }
}
