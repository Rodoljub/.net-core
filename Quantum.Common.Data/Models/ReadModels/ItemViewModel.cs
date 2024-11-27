using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Models.ReadModels
{
    public class ItemViewModel
    {
        [JsonProperty(PropertyName = "Comment")]
        public CommentViewModel CommentMostReply { get; set; }
        public int CommentsCount { get; set; }
        public string CreatedDate { get { return CreatedDateD.ToString(DateFormat); } }
        public string DateFormat { get; set; }
        public DateTime CreatedDateD { get; set; }
        public bool UserFavourite { get; set; }
        public int FavouritesCount { get; set; }
        public string ItemFilePath { get; set; }
        public FileDetailsViewModel FileDetails { get; set; }
        public string Id { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public DateTime LastModified { get; set; }
        public bool UserLiked { get; set; }
        public int LikesCount { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public int TotalCount { get; set; }
        public UserProfileEntityViewModel UserProfile { get; set; }

    }
}
