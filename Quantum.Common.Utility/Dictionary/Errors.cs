using System;
using System.Collections.Generic;
using System.Text;

namespace Quantum.Utility.Dictionary
{
    public sealed class Errors
    {
		#region User
		public const string ErrorUserEmailNotFound = "ErrorUserEmailNotFound";
        public const string ErrorRegisterEmailExist = "ErrorRegisterEmailExist";
        public const string ErrorConfirmationEmail = "ErrorConfirmationMail";
        public const string ErrorEmailNotConfirmed = "ErrorEmailNotConfirmed";
        public const string ErrorEmailNotSent = "ErrorEmailNotSent";
        public const string GeneralEmailError = "GeneralEmailError";

		public const string IdentityNotFoundUserEmailClaim = "IdentityNotFoundUserEmailClaim";
		public const string ErrorUserNotFound = "ErrorUserNotFound";

		public const string ErrorLogin = "ErrorLogin";
        public const string ErrorUser = "ErrorUser";
        public const string ErrorUserLockout = "ErrorUserLockout";
        public const string ErrorPassword = "ErrorPassword";
		#endregion User

		#region User Profile
		public const string ErrorUserProfileNotFound = "ErrorUserProfileNotFound";
        public const string ErrorUpdateProfile = "ErrorUpdateProfile";
		public const string ErrorUserProfileMaxUpdateInTime = "ErrorUserProfileMaxUpdateInTime";
		#endregion User Profile

		#region Image
		public const string ErrorProfileImage = "ErrorProfileImage";
		public const string ErrorImageFormat = "ErrorImageFormat";

		public const string ErrorImageSave = "ErrorImageSave";
		public const string ErrorAdultContentImage = "ErrorAdultContentImage";
		public const string ErrorRacyContentImage = "ErrorRacyContentImage";
		public const string ErrorAnalyzedImage = "ErrorAnalyzedImage";
		#endregion Image

		#region CLR Type
		public const string ErrorClrTypeNotFound = "ErrorClrTypeNotFound";
		#endregion CLR Type

		#region Items
		public const string ErrorItemNotFound = "ErrorItemNotFound";
		public const string ErrorItemsNotFound = "ErrorItemsNotFound";
		public const string ErrorItemNotUpdated = "ErrorItemNotUpdated";
		public const string ErrorItemWasNotDeleted = "ErrorItemWasNotDeleted";
		public const string ErrorFavouritesItemsNotFound = "ErrorFavouritesItemsNotFound";
		public const string ErrorSearchItemsNotFound = "ErrorSearchItemsNotFound";
		#endregion Items

		#region File Type
		public const string ErrorCreateFileTypeNameEmpty = "ErrorCreateFileTypeNameEmpty";
		public const string ErrorUpdateFileTypeNameEmpty = "ErrorUpdateFileTypeNameEmpty";
		public const string ErrorFileTypeNotFound = "ErrorFileTypeNotFound";
		#endregion File Type

		#region File
		public const string ErrorFileIsEmpty = "ErrorFileIsEmpty";
		public const string ErrorFileIsTooLarge = "ErrorFileIsTooLarge";
		public const string ErrorFileNotFound = "ErrorFileNotFound";
		#endregion File

		#region Comment
		public const string ErrorCommentNotDeleted = "ErrorCommentNotDeleted";
		public const string ErrorCommentNotSaved = "ErrorCommentNotSaved";
		public const string ErrorCommentsNotFound = "ErrorCommentsNotFound";
        #endregion Comment

        public const string ErrorSaveSearchTitleExist = "ErrorSaveSearchTitleExist";

        public const string ErrorExistingSaveSearch = "ErrorExistingSaveSearch";

        public const string ErrorReportedContentReasonNotFound = "ErrorReportedContentReasonNotFound";

		public const string GeneralError = "Error";


    }
}
