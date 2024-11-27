using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quantum.AuthorizationServer.Models;
using Quantum.AuthorizationServer.Models.SocialModels;
using Quantum.AuthorizationServer.Models.SocialModels.FacebookModels;
using Quantum.AuthorizationServer.Services.Contracts;
using Quantum.Utility.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services
{
    public class SocialService : ISocialService
    {
		private IConfigurationRoot _config;
		private ILogger<SocialService> _logger;
        private UserManager<IdentityUser> _userMgr;
        private IAuthService _authService;

        public SocialService(
			IConfigurationRoot config,
			ILogger<SocialService> logger,
            UserManager<IdentityUser> userMgr,
            IAuthService authService)
		{
			_config = config;
			_logger = logger;
            _userMgr = userMgr;
            _authService = authService;
        }

		private async Task<GoogleResponseModel> PostGoogleAccessToken(string code, string returnUrl)
		{
			var appIdGoogle = _config["Social:Google:AppKey"];
			var secretGoogle = _config["Social:Google:Secret"];
			var grantType = _config["Social:Google:GrantType"];
			var googleTokenUrl = _config["Social:Google:GoogleToken"];
			var alt = _config["Social:Google:Alt"];
			var googleScope = _config["Social:Google:ScopeProfile"];

			var handler = new HttpClientHandler()
			{
				AllowAutoRedirect = false,
			};

			using (var client = new HttpClient(handler))
			{
				var url = $"{googleTokenUrl}";

				var content = new FormUrlEncodedContent(new[]
				{
					new KeyValuePair<string, string>("code", code),
					new KeyValuePair<string, string>("client_id", appIdGoogle),
					new KeyValuePair<string, string>("client_secret", secretGoogle),
					new KeyValuePair<string, string>("redirect_uri", returnUrl),
					new KeyValuePair<string, string>("grant_type", grantType)
				});

				var result = await client.PostAsync(url, content);
				string resultContent = await result.Content.ReadAsStringAsync();
				var googleToken = JsonConvert.DeserializeObject<GoogleTokenModel>(resultContent);

				var userInformationUrl = $"{googleScope}?alt={alt}&access_token={googleToken.AccessToken}";

				var userInformationResponse = await client.GetStringAsync(userInformationUrl);

                _logger.LogDebug("Google social login response:", JsonConvert.SerializeObject(userInformationResponse));

                var userInformation = JsonConvert.DeserializeObject<GoogleResponseModel>(userInformationResponse);

				return userInformation;
			}
		}

		private async Task<FacebookUserInformationResponseModel> GetFacebookUserData(string code, string returnUrl)
		{
			var appId = _config["Social:Facebook:AppKey"];
			var secret = _config["Social:Facebook:Secret"];
			var facebookGraph = _config["Social:Facebook:Graph"];
			var facebookDialog = _config["Social:Facebook:Domain"];


			var handler = new HttpClientHandler()
			{
				AllowAutoRedirect = false,
			};

			using (var client = new HttpClient(handler))
			{
				var accessTokenUrl = $"{facebookGraph}oauth/access_token?code={code}&redirect_uri={returnUrl}&client_id={appId}&client_secret={secret}";

				var accessTokenResponse = await client.GetStringAsync(accessTokenUrl);

				var accessToken = JsonConvert.DeserializeObject<FacebookAccessTokenModel>(accessTokenResponse);

				var appAccessTokenUrl = $"{facebookGraph}oauth/access_token?client_id={appId}&client_secret={secret}&grant_type=client_credentials";			

				var appAccessTokenResponse = await client.GetStringAsync(appAccessTokenUrl);

				var appAccessToken = JsonConvert.DeserializeObject<FacebookAccessTokenModel>(appAccessTokenResponse);

				var debugTokenUrl = $"{facebookGraph}debug_token?input_token={accessToken.AccessToken}&access_token={appAccessToken.AccessToken}";

				var debugTokenResponse = await client.GetStringAsync(debugTokenUrl);

				var debugToken = JsonConvert.DeserializeObject<FacebookDebugTokenModel>(debugTokenResponse);

				var userPermissionsUrl = $"{facebookGraph}{debugToken.Data.User_id}/permissions?access_token={accessToken.AccessToken}";

				var userPermissionsResponse = await client.GetStringAsync(userPermissionsUrl);

				var userPermissions = JsonConvert.DeserializeObject<FacebookPermissionsModel>(userPermissionsResponse);

				var userInformationUrl = $"{facebookGraph}{debugToken.Data.User_id}?fields=email,name&access_token={accessToken.AccessToken}";

				var userInformationResponse = await client.GetStringAsync(userInformationUrl);

				var userInformation = JsonConvert.DeserializeObject<FacebookUserInformationResponseModel>(userInformationResponse);

				return userInformation;
			}
		}

        private async Task AddLoginProvider(string provider, string providerId, IdentityUser user)
        {
            if (!_userMgr.GetLoginsAsync(user).Result.Where(l => l.LoginProvider == provider).Any())
            {
                await _userMgr.AddLoginAsync(user, new UserLoginInfo(provider, providerId, provider));
            }
        }

        private async Task<SocialLoginResponseModel> GetSocialLoginResponseDataAsync(ExternalLoginModel stateData, IdentityUser user)
        {
            JwtSecurityToken tokenInfo = await _authService.CreateUserToken(user, stateData.KeepMeLoggedIn);

            var responseObject = new SocialLoginResponseModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(tokenInfo),
                Expiration = tokenInfo.ValidTo,
                ReturnUrl = stateData.LocalUrl,
                UserExists = true
            };

            return responseObject;
        }

        private  ExternalLoginModel GetExternalProviderData(string state)
        {
            return JsonConvert.DeserializeObject<ExternalLoginModel>(Encoding.UTF8.GetString(Convert.FromBase64String(state)));
        }

        public async Task<object> GetSocialLoginResponse(string code, string returnUrl, string state)
        {
            var stateData = GetExternalProviderData(state);

            switch (stateData.Provider)
            {
                case "Facebook":

                    var userInformationFacebook = await GetFacebookUserData(code, returnUrl);

                    try
                    {
                        var user = await _userMgr.FindByEmailAsync(userInformationFacebook.Email);
                        if (user == null)
                        {
                            return new SocialUserModel
                            {
                                Email = userInformationFacebook.Email,
                                Name = userInformationFacebook.Name,
                                ReturnUrl = stateData.LocalUrl,
                                UserExists = false
                            };
                        }
                        else
                        {
                            await AddLoginProvider(stateData.Provider, userInformationFacebook.Id, user);

                            return await GetSocialLoginResponseDataAsync(stateData, user);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new HttpStatusCodeException(HttpStatusCode.Unauthorized,
                            "Oops something went wrong. Please try later.",
                            "GeneralErrorMessage",
                            ex,
                            "Exception thrown while creating JWT (Facebook)");
                    }

                case "Google":

                    var userInformationGoogle = await PostGoogleAccessToken(code, returnUrl);

                    try
                    {
                        var user = await _userMgr.FindByEmailAsync(userInformationGoogle.Email);
                        if (user == null)
                        {
                            return new SocialUserModel
                            {
                                Email = userInformationGoogle.Email,
                                Name = userInformationGoogle.Name,
                                Picture = userInformationGoogle.Picture,
                                ReturnUrl = stateData.LocalUrl,
                                UserExists = false
                            };
                        }
                        else
                        {
                            await AddLoginProvider(stateData.Provider, userInformationGoogle.Id, user);

                            return await GetSocialLoginResponseDataAsync(stateData, user);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new HttpStatusCodeException(HttpStatusCode.Unauthorized,
                            "Oops something went wrong. Please try later.",
                            "GeneralErrorMessage", 
                            ex, 
                            "Exception thrown while creating JWT (Google)");
                    }

                default:
                    throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Invalid provider!");
            }
        }

        public async Task<object> GetSocialSignupResponse(string code, string returnUrl, string state)
        {
            var stateData = GetExternalProviderData(state);

            switch (stateData.Provider)
            {
                case "Facebook":

                    var userInformation = await GetFacebookUserData(code, returnUrl);

                    //Download image and send to client as base64 string 

                    return new
                    {
                        Email = userInformation.Email,
                        Name = userInformation.Name,
                        //Picture = userInformation.Picture.Data.Url,
                        ReturnUrl = stateData.LocalUrl
                    };

                case "Google":

                    var userInformationGoogle = await PostGoogleAccessToken(code, returnUrl);

                    //Download image and send to client as base64 string 

                    return new
                    {
                        Email = userInformationGoogle.Email,
                        Name = userInformationGoogle.Name,
                        Picture = userInformationGoogle.Picture,
                        ReturnUrl = stateData.LocalUrl
                    };

                default:
                    throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Invalid provider!");
            }
        }

        public string GetSocialLoginUrl(string provider, string returnUrl, string state)  
        {

            var loginUrl = string.Empty;

            switch (provider)
            {
                case "Facebook":

                    var appId = _config["Social:Facebook:AppKey"];

                    var facebookDomain = _config["Social:Facebook:Domain"];

                    loginUrl = $"{facebookDomain}?client_id={appId}&redirect_uri={returnUrl}&auth_type=rerequest&scope=email&state={state}";

                    break;

                case "Google":

                    var appIdGoogle = _config["Social:Google:AppKey"];

                    var googleDomain = _config["Social:Google:Domain"];

                    var scope = _config["Social:Google:Scope"];

                    var accessType = _config["Social:Google:AccessType"];

                    var includeGrantedScopes = _config["Social:Google:IncludeGrantedScopes"];

                    var responseType = _config["Social:Google:ResponseType"];

                    loginUrl = $"{googleDomain}?scope={scope}&access_type={accessType}&include_granted_scopes={includeGrantedScopes}&state={state}&redirect_uri={returnUrl}&response_type={responseType}&client_id={appIdGoogle}";

                    break;
            }

            return loginUrl;
        }

        public string GetExternalLoginModelState(ExternalLoginModel model)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(model)));
        }
    }
}
