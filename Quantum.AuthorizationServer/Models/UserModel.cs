using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
	/// <summary>
	/// 
	/// </summary>
	public class UserModel
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="UserModel"/> class.
		/// </summary>
		public UserModel()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserModel"/> class.
		/// </summary>
		/// <param name="email">The user email address [examole@somrthing.com].</param>
		/// <param name="name">The name.</param>
		/// <param name="userImage">The user profile image [base64 string].</param>
		/// <param name="emailConfirmed">if set to <c>true</c> [email is confirmed].</param>
		public UserModel(string email, string name, string userImage, bool emailConfirmed)
		{
			this.Email = email;
			this.Name = name;
			this.UserImage = userImage;
			this.EmailConfirmed = emailConfirmed;
		}

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the user image.
		/// </summary>
		/// <value>
		/// The user image.
		/// </value>
		public string UserImage { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [email confirmed].
		/// </summary>
		/// <value>
		///   <c>true</c> if [email confirmed]; otherwise, <c>false</c>.
		/// </value>
		public bool EmailConfirmed { get; set; }
	}
}
