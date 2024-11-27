using Quantum.AuthorizationServer.Models;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services.Contracts
{
    public interface ISocialService
    {
        Task<object> GetSocialLoginResponse(string code, string returnUrl, string state);

        Task<object> GetSocialSignupResponse(string code, string returnUrl, string state);

        string GetSocialLoginUrl(string provider, string returnUrl, string state);

        string GetExternalLoginModelState(ExternalLoginModel model);
    }
}
