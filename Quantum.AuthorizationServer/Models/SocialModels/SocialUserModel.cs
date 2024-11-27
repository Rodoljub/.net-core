using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models.SocialModels
{
    public class SocialUserModel
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public string Picture { get; set; }

        public string ReturnUrl { get; set; }

        public bool UserExists { get; set; }
    }
}
