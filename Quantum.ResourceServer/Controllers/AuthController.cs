using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Quantum.Core.Models.Auth;
using Quantum.Core.Services.Auth.Contracts;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Filters;
using Quantum.Utility.Services.Contracts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.AuthorizationServer.Controllers
{
    [EnableCors("GeneralPolicy")]
    [Produces("application/json")]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        #region Declarations

        private ILogger<AuthController> _logger;
        private UserManager<IdentityUser> _userMgr;
        private IUserManagerService _userManagerService;
        private IConfiguration _config;
        private IEmailService _emailServ;
        private IUserProfileService _userProfileServ;
        private IUserProfileRepository _userProfileRepo;
        private IDocumentService _docServ;
        private IUtilityService _utilServ;


        #endregion

        public AuthController(

            UserManager<IdentityUser> userMgr,
            IUserManagerService userManagerService,
            ILogger<AuthController> logger,
            IEmailService emailService,
            IConfiguration config,
            IUserProfileService userProfileServ,
            IUserProfileRepository userProfileRepo,
            IDocumentService docServ,
            IUtilityService utilServ
        )
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
            _userManagerService = userManagerService ?? throw new ArgumentNullException(nameof(userManagerService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _emailServ = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _userProfileServ = userProfileServ ?? throw new ArgumentNullException(nameof(userProfileServ));
            _userProfileRepo = userProfileRepo ?? throw new ArgumentNullException(nameof(userProfileRepo));
            _docServ = docServ ?? throw new ArgumentNullException(nameof(docServ));
            _utilServ = utilServ ?? throw new ArgumentNullException(nameof(utilServ));
        }

        [ValidateModel]
        [HttpPost("validateregister")]
        public IActionResult ValidateRegister([FromBody]RegisterModel model)
        {
            return Ok(model);
        }

        [ValidateRecaptcha]
        [ValidateModel]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm]RegisterModel model)
        {
            var createUser = await _userManagerService.CreateUser(model);

            if (createUser.IdentityResult != IdentityResult.Success)
            {
                _logger.LogError("The email: {0} is already exist", model.Email);
                ModelState.AddModelError(Errors.ErrorRegisterEmailExist, $"The email: \"{model.Email}\" is already register.");
                return BadRequest(ModelState);
            }

            //var urlSegment = await _userProfileServ.CreateRandomUrlSegment(model.Email);
            //if (await _userProfileServ.UrlSegmentExists(model.Email))
            //{
            //    _logger.LogError($"The name: {model.Email} , already exist");
            //    ModelState.AddModelError("ErrorRegisterNameExist", $"The name: \"{model.Email}\" is not available. :(");
            //    return BadRequest(ModelState);
            //}

            await _userProfileServ.CreateUserProfile(model, createUser.User);

            await _userManagerService.CreateUserClaims(createUser.User, model);

            var emailTemplate = await _docServ.GetFileContentAsString(FileTypes.Emails.ConfirmEmailTemplate);

            var confirmEmailUrl = await _emailServ.GetConfirmEmailUrl(model.Email, model.ReturnUrl, this.Url/*, this.Request*/);

            //bool emailSent =
                await _emailServ.SendConfirmationEmail(model.Email, model.Name, confirmEmailUrl, emailTemplate);

            //if (!emailSent)
            //{
            //    _logger.LogError($"Conformation Email for user: {0}, was not send", createUser.User.Id);
            //    ModelState.AddModelError(Errors.ErrorConfirmationEmail, "Confirmation email was not send, please resend confirmation email.");
            //    return BadRequest(ModelState);
            //}

            return Ok();

            //return Created(Url.Link("UserDetailsRoute", null), new UserModel
            //{
            //    Name = model.Name,
            //    Email = createUser.User.Email,
            //    EmailConfirmed = createUser.User.EmailConfirmed
            //});
        }

        //[Authorize(LocalApi.PolicyName)]
        //[HttpPost("userdetails", Name = "UserDetailsRoute")]
        //public async Task<IActionResult> GetUser()
        //{
        //    var user = await _utilServ.GetAuthUser(User.Identity);

        //    if (user == null)
        //    {
        //        _logger.LogError("User cannot be found for the principal: {0}", User.Identity.Name);

        //        ModelState.AddModelError(Errors.ErrorUser, "User cannot be found.");
        //        return BadRequest(ModelState);
        //    }

        //    var userModel = await _userProfileRepo.GetUserProfileByEmail(user.Email, getProfileImage: true);

        //    return Ok(userModel);
        //}

        [ValidateRecaptcha]
        [ValidateModel]
        [HttpPost("ReSendConfirmationEmail")]
        public async Task<IActionResult> ReSendConfirmationEmail([FromForm] EmailModel model)
        {
            var name = (await _userProfileRepo.GetUserProfileByEmail(model.Email))?.Name;

            var emailTemplate = await _docServ.GetFileContentAsString(FileTypes.Emails.ConfirmEmailTemplate);

            var confirmEmailUrl = await _emailServ.GetConfirmEmailUrl(model.Email, model.ReturnUrl, this.Url/*, this.Request*/);

            await _emailServ.SendConfirmationEmail(model.Email, name, confirmEmailUrl, emailTemplate);

            //if (emailSent)
            //{
                return Ok();
            //}
            
            //_logger.LogError($"Conformation Email for user: {0}, was not sent", model.Email);
            //ModelState.AddModelError(Errors.ErrorConfirmationEmail, "Confirmation email was not sent, please resend confirmation email.");
            //return BadRequest(ModelState);
        }

        [ValidateModel]
        [HttpPost("validateEmail")]
        public IActionResult ValidateEmail([FromBody] EmailModel model)
        {
            return Ok(model);
        }


        [AllowAnonymous]
        [HttpPost("confirmemail", Name = "ConfirmEmailRoute")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailModel model)
        {
            var user = await _userMgr.FindByIdAsync(model.userId);

            if (user == null)
            {
                _logger.LogWarning("User cannot be found for Id: {0}", model.userId);

                ModelState.AddModelError(Errors.ErrorUser, "User cannot be found.");
                return BadRequest(ModelState);
            }

            IdentityResult result = await _userMgr.ConfirmEmailAsync(user, model.token);

            if (result.Succeeded)
            {
                return Ok();
        }
            else
            {
                return BadRequest(result);
            }
        }

        //[ValidateRecaptcha]
        [ValidateModel]
        [HttpPost("sendresetpasswordemail")]
        public async Task<IActionResult> SendResetPassEmail([FromForm] EmailModel model)
        {
            var resetPassUrl = await _emailServ.GetResetPasswordUrl(model.Email, this.Url);

            var emailTemplate = await _docServ.GetFileContentAsString(FileTypes.Emails.ResetPasswordEmail);

            var name = (await _userProfileRepo.GetUserProfileByEmail(model.Email))?.Name;

            //var emailSent = 
                await _emailServ.SendResetPasswordEmail(model.Email, name, resetPassUrl, emailTemplate);

            //if (emailSent)
            //{
                return Ok();
            //}

            //ModelState.AddModelError(Errors.ErrorEmailNotSent, "Email was not sent, please try again later.");
            //return BadRequest(ModelState);
        }

        [ValidateModel]
        [HttpPost("resetpassword", Name = "ResetPasswordRoute")]
        public async Task<IActionResult> ResetPassword([FromQuery] ResetPasswordTokenModel model, [FromBody] ResetPasswordModel newmodel)
        {
            var user = await _userMgr.FindByIdAsync(model.UserId);

            if (user == null)
            {
                _logger.LogError($"User cannot be found for UserId: {0}", model.UserId);
                ModelState.AddModelError(Errors.ErrorUser, "User cannot be found.");
                return BadRequest(ModelState);
            }

            IdentityResult result = await _userMgr.ResetPasswordAsync(user, model.Token, newmodel.Password);

            if (result.Succeeded)
            {
                var locoutDateTime = DateTime.UtcNow.AddDays(-1);
                await _userMgr.SetLockoutEndDateAsync(user, locoutDateTime);
                await _userMgr.ResetAccessFailedCountAsync(user);
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        [Authorize(LocalApi.PolicyName)]
        [ValidateModel]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _utilServ.GetAuthUser(User.Identity);

            if (user == null)
            {
                _logger.LogError("User cannot be found for the principal: {0}", User.Identity.Name);
                ModelState.AddModelError(Errors.ErrorUser, "User cannot be found.");
                return BadRequest(ModelState);
            }

            if (model.CurrentPassword.Trim() == model.Password.Trim())
            {
                ModelState.AddModelError(Errors.ErrorPassword, "Same password cannot be reused!");
                return BadRequest(ModelState);
            }

            IdentityResult result = await _userMgr.ChangePasswordAsync(user, model.CurrentPassword, model.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

    }
}
