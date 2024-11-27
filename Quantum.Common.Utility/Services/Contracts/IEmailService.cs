using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Utility.Services.Contracts
{
    public interface IEmailService
    {
        Task SendResetPasswordEmail(string emailTo, string name, string resetPasswordEmailUrl, string htmlTemplate);

        Task SendConfirmationEmail(string emailTo, string name, string confirmationUrl, string htmlTemplate);

        Task<string> GetConfirmEmailUrl(string userEmail, string returnUrl, IUrlHelper Url/*, HttpRequest request*/);

        Task<string> GetResetPasswordUrl(string userEmail, IUrlHelper Url);
    }
}
