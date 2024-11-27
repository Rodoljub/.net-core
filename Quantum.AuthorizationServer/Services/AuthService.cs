using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Quantum.AuthorizationServer.Services.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services
{

    public class AuthService : IAuthService
    {
        private UserManager<IdentityUser> _userMgr;
        private IMemoryCache _cache;
        private ILogger<AuthService> _logger;
        private IConfigurationRoot _config;
        private IPasswordHasher<IdentityUser> _hasher;
        private RoleManager<IdentityRole> _roleMgr;
        private IUtilityService _utilServ;

        public AuthService(
            UserManager<IdentityUser> userMgr,
            IMemoryCache cache,
            ILogger<AuthService> logger,
            IConfigurationRoot config,
            IPasswordHasher<IdentityUser> hasher,
            RoleManager<IdentityRole> roleMgr,
            IUtilityService utilServ

            )
        {
            _userMgr = userMgr;
            _cache = cache;
            _logger = logger;
            _config = config;
            _hasher = hasher;
            _roleMgr = roleMgr;
            _utilServ = utilServ;

        }


        public bool CheckRefrashToken(IIdentity userIdentity)
        {
            var identity = userIdentity as ClaimsIdentity;

            var expires = identity.Claims.FirstOrDefault(c => c.Type == "exp").Value;

            if (double.TryParse(expires, out var expDate))
            {
                var expDateTime = _utilServ.ConvertFromUnixTimestamp(expDate);

                var tokenExpiresSoon = TokenExpiresSoon(expDateTime);

                return tokenExpiresSoon;
            }

            return false;
        }

        private bool TokenExpiresSoon(DateTime expDateTime)
        {
            //var expDateTime = GetTokenExpieryDateTime(appToken);
            var dateTimeUtNow = DateTime.UtcNow;
            var validMaxDateTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Tokens:ValidMaxTameTokenExpiresSoon"]));

            return (expDateTime > dateTimeUtNow && expDateTime <= validMaxDateTime);
        }

        public async Task<(bool Authenticated, string Error, IdentityUser User)> AuthenticateUser(string username, string password)
        {
            bool authenticated = false;
            string error = null;

            var user = await _userMgr.FindByEmailAsync(username);

            if (user != null)
            {
                if (_hasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success)
                {
                    if (user.LockoutEnd <= DateTime.UtcNow || user.LockoutEnd == null)
                    {
                        await _userMgr.ResetAccessFailedCountAsync(user);

                        if (user.EmailConfirmed)
                        {
                            authenticated = true;
                        }
                        else
                        {
                            error = Errors.ErrorEmailNotConfirmed;
                            _logger.LogWarning("EmailNotConfirmed for user: {0}", username);
                        }
                    }
                    else
                    {
                        error = Errors.ErrorUserLockout;
                        _logger.LogWarning("UserLockout: {0}", username);
                    }
                }
                else
                {
                    error = Errors.ErrorLogin;
                    _logger.LogDebug("LoginFailed : {0}", username);
                }
            }
            else
            {
                error = Errors.ErrorLogin;
                _logger.LogDebug("User cannot be found for the email: {0}", username);
            }

            return (Authenticated: authenticated, Error: error, User: user);
        }

        public async Task ValidateLoginAttempt(IdentityUser User)
        {
            var maxFailedAccessAttempts = _config.GetAsInteger("User:Lockout:MaxFailedAccessAttempts", 3);

            if (maxFailedAccessAttempts <= (await _userMgr.GetAccessFailedCountAsync(User) + 1))
            {
                var lockoutTimeSpan = _config.GetAsDouble("User:Lockout:DefaultLockoutTimeSpan", 5);

                var locoutDateTime = DateTime.UtcNow.AddMinutes(lockoutTimeSpan);
                await _userMgr.SetLockoutEndDateAsync(User, locoutDateTime);
            }
            else
            {
                await _userMgr.AccessFailedAsync(User);
            }
        }

        public async Task<JwtSecurityToken> CreateUserToken(IdentityUser user, bool keepMeLoggedIn)
        {
            var userClaims = await _userMgr.GetClaimsAsync(user);

            var userRoles = await _userMgr.GetRolesAsync(user);

            var userRoleClaims = userRoles
                .Select(async roleNmae => await _roleMgr.FindByNameAsync(roleNmae))
                .Select(async userRole => await _roleMgr.GetClaimsAsync(await userRole))
                .SelectMany(cls => cls.Result);

            var claims = new List<Claim>()
            {
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }
            .Union(userClaims)
            .Union(userRoleClaims)
            .Distinct()
            .ToList();

            if (!claims.Any(claim => claim.Type == JwtRegisteredClaimNames.Email))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime? expires = null;

            if (keepMeLoggedIn == true)
            {
                expires = DateTime.UtcNow.AddDays(_config.GetAsInteger("Tokens:KeepMeLoggednExpires", 7));
            }
            else
            {
                expires = DateTime.UtcNow.AddMinutes(_config.GetAsInteger("Tokens:Expires", 20));
            }

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: _config["Tokens:Issuer"],
               audience: _config["Tokens:Audience"],
               claims: claims,
               expires: expires,
               signingCredentials: creds
             );

            return token;
        }




        //public async Task<string> GetOrRefreshAppTokenAsync(string appName, string secret, string appToken, string authServiceBaseUrl)
        //{
        //	if (NewKeyRequired(appToken))
        //	{
        //		appToken = await GetDeviceTokenAsync(appName, secret, authServiceBaseUrl);
        //	}
        //	else
        //	{
        //		if (TokenExpiresSoon(appToken))
        //		{
        //			Task.Factory.StartNew(() => GetAppRefreshTokenAsync(appToken, authServiceBaseUrl));


        //		}
        //	}

        //	return appToken;
        //}

        //public async Task GetAppRefreshTokenAsync(string token, string authServiceBaseUrl)
        //{
        //	var handler = new HttpClientHandler()
        //	{
        //		AllowAutoRedirect = false,
        //	};

        //	using (var client = new HttpClient(handler))
        //	{
        //		var url = $"{authServiceBaseUrl}/api/auth/device/refreshtoken";

        //		if (token != null)
        //		{
        //			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //		}

        //		var response = await client.PostAsync(url, null);

        //		if (response.IsSuccessStatusCode)
        //		{
        //			var result = await response.Content.ReadAsStringAsync();
        //			var data = JsonConvert.DeserializeObject<TokenModel>(result);
        //			_cache.Set("ApplicationToken", data.token);

        //		}
        //		else
        //		{
        //			_logger.LogError($"'GetApplicationRefreshToken' was not successful. Returned status code was: '{response.StatusCode }.");
        //		}
        //	}
        //}



        //private bool NewKeyRequired(string appToken)
        //{
        //	if (string.IsNullOrWhiteSpace(appToken))
        //	{
        //		return true;
        //	}
        //	else
        //	{
        //		var expDateTime = GetTokenExpieryDateTime(appToken);
        //		var validDateTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Tokens:ValidateTimeAdd"]));

        //		if (validDateTime >= expDateTime)
        //		{
        //			return true;
        //		}
        //	}

        //	return false;
        //}



        //private DateTime GetTokenExpieryDateTime(string jwt)
        //{
        //	var jwtToken = new JwtSecurityToken(jwt);
        //	if (jwtToken != null && jwtToken.Claims != null)
        //	{
        //		var expiresClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

        //		if (expiresClaim != null)
        //		{
        //			var expiresValue = 0D;
        //			if (double.TryParse(expiresClaim.Value, out expiresValue))
        //			{
        //				return ConvertFromUnixTimestamp(expiresValue);
        //			}
        //		}
        //	}

        //	var returnDate = DateTime.Now.AddDays(Convert.ToInt32(_config["Tokens:BadTokenReturnDateTime"]));
        //	return returnDate;
        //}

        //private async Task<string> GetDeviceTokenAsync(string appName, string secret, string authServiceBaseUrl)
        //{
        //	var url = $"{authServiceBaseUrl}/api/auth/device/token";
        //	var result = await GetApplicationTokenAsync(appName, secret, url);

        //	if (!string.IsNullOrWhiteSpace(result))
        //	{
        //		var data = JsonConvert.DeserializeObject<TokenModel>(result);

        //		if (data != null && !string.IsNullOrWhiteSpace(data.token))
        //		{
        //			//var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal);
        //			_cache.Set("ApplicationToken", data.token/*, entryOptions*/);

        //			return data.token;
        //		}
        //	}

        //	return null;
        //}

        //private class TokenModel
        //{
        //	public string token { get; set; }
        //}

        //private async Task<string> GetApplicationTokenAsync(string appName, string secret, string url)
        //{
        //	string result = null;

        //	var handler = new HttpClientHandler()
        //	{
        //		AllowAutoRedirect = false,
        //	};

        //	using (var client = new HttpClient(handler))
        //	{
        //		var content = new FormUrlEncodedContent(new[]
        //		{
        //			new KeyValuePair<string, string>("AppName", appName),
        //			new KeyValuePair<string, string>("Secret", secret)
        //		});

        //		var response = await client.PostAsync(url, content);

        //		if (response.IsSuccessStatusCode)
        //		{
        //			result = await response.Content.ReadAsStringAsync();
        //		}
        //		else
        //		{
        //			_logger.LogError($"'GetApplicationTokenAsync' was not successful. Returned status code was: '{response.StatusCode }.");
        //		}
        //	}

        //	return result;
        //}


    }

}
