using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Core.Models.UserProfile
{
    public class UserProfileModel
    {
		[Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
		[MinLength(4, ErrorMessage = "Minimum length is 4 characters")]
		[MaxLength(25, ErrorMessage = "Maximum length is 25 characters")]
		public string Name { get; set; }

		public string UserImage { get; set; }
	}
}
