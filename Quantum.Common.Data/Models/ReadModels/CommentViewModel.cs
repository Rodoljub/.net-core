using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Models.ReadModels
{
    public class CommentViewModel
    {
        public int ChildCount { get; set; }
        public string Content { get; set; }
        public string CreatedDate { get { return CreatedDateD.ToString(DateFormat); } }
        public string DateFormat { get; set; }
        public DateTime CreatedDateD { get; set; }
        public string ID { get; set; }
        public int LikeCount { get; set; }
        public bool UserLiked { get; set; }
        public UserProfileEntityViewModel UserProfile { get; set; }
    }
}
