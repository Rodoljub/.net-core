using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models.UserProfile
{
	public class UserProfileViewModel
	{
		public string Name { get; set; }

		public string ImagePath { get; set; }

		public int UploadsCount { get; set; }

		public int CommentsCount { get; set; }

		public int LikesCount { get; set; }

		public int FavouritesCount { get; set; }

        public AnalyzingImageViewModel AnalyzingImageViewModel { get; set; }
    }
}
