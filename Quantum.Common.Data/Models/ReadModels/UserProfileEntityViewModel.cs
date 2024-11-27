using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Models.ReadModels
{
    public class UserProfileEntityViewModel
    {
        public string Name { get; set; }
        public string UrlSegment { get; set; }
        public string UserImageFileId { get; set; }
        public string UserImageFileExtansion { get; set; }
        public string UserImagePath { get; set; }
        public bool UserEntityOwner { get; set; }
    }
}
