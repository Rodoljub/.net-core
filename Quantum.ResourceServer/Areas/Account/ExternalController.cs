using IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quantum.Core.Models.Auth;
using Quantum.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    //[SecurityHeaders]
    [ServiceFilter(typeof(SecurityHeadersAttribute))]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserManagerService _userManagerService;
        private readonly IUserProfileService _userProfileServ;
        private IConfiguration _config;
        //private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly ILogger<ExternalController> _logger;
        private readonly IEventService _events;

        public ExternalController(
            UserManager<IdentityUser> userManager,
            IUserManagerService userManagerService,
            IUserProfileService userProfileServ,
            IConfiguration config,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            ILogger<ExternalController> logger//,
            //TestUserStore users = null
            )
        {
            _userManager = userManager;
            _userManagerService = userManagerService;
            _userProfileServ = userProfileServ;
            _config = config;
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
            //_users = users ?? new TestUserStore(TestUsers.Users);

            _interaction = interaction;
            _clientStore = clientStore;
            _logger = logger;
            _events = events;
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult Challenge(string scheme, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            returnUrl = $"{_config["FrontApp:Domain"]}{_config["FrontApp:ExternalSigninCallback"]}";
            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)), 
                Items =
                {
                    { "returnUrl", returnUrl }, 
                    { "scheme", scheme },
                }
            };

            return Challenge(props, scheme);
            
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@claims}", externalClaims);
            }

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = await FindUserFromExternalProvider(result);
            if (user == null)
            {
                var email = claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
                user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));

                    //if (!_userManager.GetLoginsAsync(user.User).Result.Where(l => l.LoginProvider == provider).Any())
                    //{
                    //    await _userManager.AddLoginAsync(user.User, new UserLoginInfo(provider, providerUserId, provider));
                    //}
                }
                else
                {
                    // this might be where you might initiate a custom workflow for user registration
                    // in this sample we don't show how that would be done, as our sample implementation
                    // simply auto-provisions new external user
                    user = await AutoProvisionUser(provider, providerUserId, claims);
                }
            }

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);
            
            // issue authentication cookie for user
            var isuser = new IdentityServerUser(user.Id)
            {
                DisplayName = user.UserName,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, user.UserName, true, context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", returnUrl);
                }
            }

            return Redirect(returnUrl);
        }

        private async Task<(IdentityUser user, string provider, string providerUserId, IEnumerable<Claim> claims
            )> FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            //var user = _users.FindByExternalProvider(provider, providerUserId);
            var user = await _userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        private async Task<IdentityUser> AutoProvisionUser(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            
            var email = claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var name = claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;
            //var user = _users.AutoProvisionUser(provider, providerUserId, claims.ToList());
            var model = new RegisterModel()
            {
                Name = name,
                Email = email,
                Password = GenerateRandomPassword(),
                ReturnUrl = ""
            };
            var user = await _userManagerService.CreateUser(model, true);

            await _userProfileServ.CreateUserProfile(model, user.User);

            await _userManagerService.CreateUserClaims(user.User, model);

            await _userManager.AddLoginAsync(user.User, new UserLoginInfo(provider, providerUserId, provider));

            //if (!_userManager.GetLoginsAsync(user.User).Result.Where(l => l.LoginProvider == provider).Any())
            //{
            //    await _userManager.AddLoginAsync(user.User, new UserLoginInfo(provider, providerUserId, provider));
            //}

            return user.User;
        }

        // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
        // this will be different for WS-Fed, SAML2p or other protocols
        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }

        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        public string GenerateRandomPassword(PasswordOptions opts = null)
        {
 
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = Convert.ToInt32(_config["User:Password:RequiredLength"]), 
                //RequiredUniqueChars = 4,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireNonAlphanumeric = true,
                //RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}