using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Quantum.AuthorizationServer.Models;
using Quantum.AuthorizationServer.Services.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Filters;
using Quantum.Utility.Services.Contracts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Controllers
{
    [EnableCors("GeneralPolicy")]
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        #region Declarations

        private ILogger<AuthController> _logger;
        private UserManager<IdentityUser> _userMgr;
        private IConfigurationRoot _config;
        private IEmailService _emailServ;
        private IAuthService _authServ;
        private ISocialService _socialServ;
        private IUserProfileService _userProfileServ;
        private IDocumentService _docServ;
        private IUtilityService _utilServ;


        #endregion

        public AuthController(

            UserManager<IdentityUser> userMgr,
            ILogger<AuthController> logger,
            IEmailService emailService,
            Services.Contracts.IAuthService authServ,
            IConfigurationRoot config,
            ISocialService socialServ,
            IUserProfileService userProfileServ,
            IDocumentService docServ,
            IUtilityService utilServ
        )
        {

            _logger = logger;
            _userMgr = userMgr;
            _config = config;
            _emailServ = emailService;
            _authServ = authServ;
            _socialServ = socialServ;
            _userProfileServ = userProfileServ;
            _docServ = docServ;
            _utilServ = utilServ;
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
            if (await _userProfileServ.UrlSegmentExists(model.Name))
            {
                _logger.LogError($"The name: {model.Name} , already exist");
                ModelState.AddModelError("ErrorRegisterNameExist", $"The name: \"{model.Name}\" is not available. :(");
                return BadRequest(ModelState);
            }

            var createUser = await _userProfileServ.CreateUser(model);

            if (createUser.IdentityResult != IdentityResult.Success)
            {
                _logger.LogError("The email: {0} is already exist", model.Email);
                ModelState.AddModelError(Errors.ErrorRegisterNameExist, $"The email: \"{model.Email}\" is already taken. :(");
                return BadRequest(ModelState);
            }

			await _userProfileServ.CreateUserProfile(model, createUser.User);

            await _userProfileServ.CreateUserClaims(createUser.User, model);

            var emailTemplate = await _docServ.GetFileContentAsString(FileTypes.Emails.ConfirmEmailTemplate);

            var confirmEmailUrl = await _emailServ.GetConfirmEmailUrl(model.Email, model.ReturnUrl, this.Url/*, this.Request*/);

            bool emailSent = await _emailServ.SendConfirmationEmail(model.Email, model.Name, confirmEmailUrl, emailTemplate);

            if (!emailSent)
            {
                _logger.LogError($"Conformation Email for user: {0}, was not send", createUser.User.Id);
                ModelState.AddModelError(Errors.ErrorConfirmationEmail, "Confirmation email was not send, please resend confirmation email.");
                return BadRequest(ModelState);
            }

            return Created(Url.Link("UserDetailsRoute", null), new UserModel
            {
                Name = model.Name,
                Email = createUser.User.Email,
                EmailConfirmed = createUser.User.EmailConfirmed
            });
        }

        [ValidateRecaptcha]
        [ValidateModel]
        [HttpPost("ReSendConfirmationEmail")]
        public async Task<IActionResult> ReSendConfirmationEmail([FromForm] EmailModel model)
        {
            var name = (await _userProfileServ.GetUserProfileByEmail(model.Email))?.Name;

            var emailTemplate = await _docServ.GetFileContentAsString(FileTypes.Emails.ConfirmEmailTemplate);

            var confirmEmailUrl = await _emailServ.GetConfirmEmailUrl(model.Email, model.ReturnUrl, this.Url/*, this.Request*/);

            var emailSent = await _emailServ.SendConfirmationEmail(model.Email, name, confirmEmailUrl, emailTemplate);

            if (emailSent)
            {
                return Ok();
            }
            
            _logger.LogError($"Conformation Email for user: {0}, was not sent", model.Email);
            ModelState.AddModelError(Errors.ErrorConfirmationEmail, "Confirmation email was not sent, please resend confirmation email.");
            return BadRequest(ModelState);
        }

        [ValidateModel]
        [HttpPost("validateEmail")]
        public IActionResult ValidateEmail([FromBody] EmailModel model)
        {
            return Ok(model);
        }

        [ValidateModel]
        [HttpPost("token")]
        public async Task<IActionResult> CreateToken([FromBody] LoginModel model)
        {
            var authResult = await _authServ.AuthenticateUser(model.Email, model.Password);

            if (authResult.Authenticated)
            {
                JwtSecurityToken token = await _authServ.CreateUserToken(authResult.User, model.KeepMeLoggedIn);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            else
            {
                switch (authResult.Error)
                {
                    case Errors.ErrorEmailNotConfirmed:

                        ModelState.AddModelError(authResult.Error, "Email adderess in still not confirmed, please confirm your email.");
                        return BadRequest(ModelState);

                    case Errors.ErrorUserLockout:

                        ModelState.AddModelError(authResult.Error, "User is locked. please try later of reset you password.");
                        return BadRequest(ModelState);

                    case Errors.ErrorLogin:

                        if (authResult.User != null)
                        {
                            await _authServ.ValidateLoginAttempt(authResult.User);
                        }

                        ModelState.AddModelError(authResult.Error, "Email or Password is invalid.");
                        return BadRequest(ModelState);
                }

                ModelState.AddModelError(authResult.Error, "Email or Password is invalid.");
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var user = await _utilServ.GetAuthUser(User.Identity);

                if (user == null)
                {
                    _logger.LogError("User cannot be found for the principal: {0}", User.Identity.Name);
                    ModelState.AddModelError(Errors.ErrorLogin, "Email or Password is invalid");
                    return BadRequest(ModelState);
                }
                else
                {
                    if (_authServ.CheckRefrashToken(User.Identity))
                    {
                        bool keepMeLoggedIn = false;

                        JwtSecurityToken token = await _authServ.CreateUserToken(user, keepMeLoggedIn);

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while creating JWT: {ex}");
            }

            ModelState.AddModelError(Errors.ErrorLogin, "Refresh token creation faild!");
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost("userdetails", Name = "UserDetailsRoute")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _utilServ.GetAuthUser(User.Identity);

            if (user == null)
            {
                _logger.LogError("User cannot be found for the principal: {0}", User.Identity.Name);

                ModelState.AddModelError(Errors.ErrorUser, "User cannot be found.");
                return BadRequest(ModelState);
            }

            var userModel = await _userProfileServ.GetUserProfileByEmail(user.Email, getProfileImage: true);

            return Ok(userModel);
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
                JwtSecurityToken token = await _authServ.CreateUserToken(user, false);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    returnUrl = model.ReturnUrl
                });
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

            var name = (await _userProfileServ.GetUserProfileByEmail(model.Email))?.Name;

            var emailSent = await _emailServ.SendResetPasswordEmail(model.Email, name, resetPassUrl, emailTemplate);

            if (emailSent)
            {
                return Ok();
            }

            ModelState.AddModelError(Errors.ErrorEmailNotSent, "Email was not sent, please try again later.");
            return BadRequest(ModelState);
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

        [Authorize]
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

        [HttpPost("ExternalLogin")]
        [AllowAnonymous]
        public IActionResult ExternalLogin([FromBody] ExternalLoginModel model)
        {
            string state = _socialServ.GetExternalLoginModelState(model);

            var newLoginUrl = _socialServ.GetSocialLoginUrl(model.Provider, model.ReturnUrl, state);

            return Ok(new { LoginUrl = newLoginUrl });
        }

        [AllowAnonymous]
        [HttpPost("SocialSignup")]
        public async Task<IActionResult> SocialSignup([FromQuery]string code, string returnUrl, string state)
        {
            var response = await _socialServ.GetSocialSignupResponse(code, returnUrl, state);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("SocialLogin")]
        public async Task<IActionResult> SocialLogin([FromQuery]string code, string returnUrl, string state)
        {
            var response = await _socialServ.GetSocialLoginResponse(code, returnUrl, state);

            return Ok(response);
        }
    }
}
