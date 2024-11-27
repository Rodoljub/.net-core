using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using Quantum.Utility.Services.Contracts;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Quantum.Utility.Services
{
    public class EmailService : IEmailService
    {
        private ILogger<EmailService> _logger;
        private IConfiguration _config;
        private IUtilityService _utilityServ;
        private UserManager<IdentityUser> _userMgr;

        public EmailService(IConfiguration config,
                            ILogger<EmailService> logger,
                            IUtilityService utilityServ,
                            UserManager<IdentityUser> userMgr)
        {
            _config = config;
            _logger = logger;
            _utilityServ = utilityServ;
            _userMgr = userMgr;
        }
        private Task SendMessage(MailAddress from, string emailTo, string subject, string htmlMessage)
        {

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            //create the mail message 
            MailMessage mail = new MailMessage();

            var fromAddress = _config.GetSection("SMTP:FromAddress").Value;
            var server = _config.GetSection("SMTP:Server").Value;
            var pass = _config.GetSection("SMTP:Password").Value;

            //set the addresses 
            mail.From = new MailAddress(fromAddress); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(emailTo);

            //set the content 
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = htmlMessage;
            //send the message 
            SmtpClient smtp = new SmtpClient(server);

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential(fromAddress, pass);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = Credentials;
            smtp.Port = 25;    //alternative port number is 8889
            smtp.EnableSsl = false;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace, ex.Message);
                throw;
            }

            return Task.CompletedTask;
        }

        private Task SendMessage(MailAddress to, string subject, string htmlMessage)
        {
            var from = new MailAddress(_config["Email:Sender:Address"], _config["Email:Sender:Name"]);
            return SendMessage(from, to.Address, subject, htmlMessage);
        }

        public Task SendResetPasswordEmail(string emailTo, string name, string resetPasswordEmailUrl, string htmlTemplate)
        {
            string resetUrl = _utilityServ.EncodeUrl(resetPasswordEmailUrl);

            htmlTemplate = htmlTemplate
                .Replace("{Name}", name)
                .Replace("{domain}", _config["FrontApp:Domain"])
                .Replace("{images}", _config["FrontApp:Images"])
                .Replace("{AppName}", _config["Application:Name"])
                .Replace("{resetUrl}", resetUrl)
                .Replace("{datetimenow}", DateTime.UtcNow.ToString());

            _logger.LogInformation($"ResetPasswordEmail for user: '{emailTo}' - was sent to SMTP server");
            var response = this.SendMessage(new MailAddress(emailTo, name), "Reset Password", htmlTemplate);

            //var result = response.StatusCode == HttpStatusCode.Accepted;

            //if (!result)
            //{
            //    _logger.LogError($"ResetPasswordEmail for user: '{emailTo}' failed due to following reason: {await response.Body.ReadAsStringAsync()}");
            //}

            //return result;

            return Task.CompletedTask;
        }

        public Task SendConfirmationEmail(string emailTo, string name, string confirmationUrl, string htmlTemplate)
        {
            var to = new MailAddress(emailTo, name);

            string confirmUrl = _utilityServ.EncodeUrl(confirmationUrl);

            htmlTemplate = htmlTemplate
                .Replace("{Name}", name)
                .Replace("{domain}", _config["FrontApp:Domain"])
                .Replace("{images}", _config["FrontApp:Images"])
                .Replace("{AppName}", _config["Application:Name"])
                .Replace("{confirmUrl}", confirmUrl)
                .Replace("{datetimenow}", DateTime.UtcNow.ToString());


            return SendMessage(to, "Successfull registration", htmlTemplate);
        }

        public async Task<string> GetConfirmEmailUrl(string userEmail, string returnUrl, IUrlHelper Url /*, HttpRequest request */)
        {
            var user = await _userMgr.FindByEmailAsync(userEmail);

            if (user == null)
            {

            }

            var token = await _userMgr.GenerateEmailConfirmationTokenAsync(user);

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogError("Email confirmation token is not generated for user: {0}", user.Id);
            }

            var backendConfirmEmailUrl = Url.Link("ConfirmEmailRoute", new { userId = user.Id, token, returnUrl });

            //var clientApplicationUri = new Uri(request.GetEncodedUrl());
            //var clientApplicationAuthority = string.Empty;

            //if (clientApplicationUri != null)
            //{
            //    clientApplicationAuthority = clientApplicationUri.Authority;
            //}

            var confirmEmailUrl = $"{_config["FrontApp:Domain"]}{_config["FrontApp:ConfirEmailUrl"]}{QueryString.Create("confirmurl", backendConfirmEmailUrl).ToUriComponent()}";
            return confirmEmailUrl;
        }

        public async Task<string> GetResetPasswordUrl(string userEmail, IUrlHelper Url)
        {
            var user = await _userMgr.FindByEmailAsync(userEmail);

            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                           $"User cannot be found for email: '{userEmail}'.",
                           Errors.ErrorUserEmailNotFound,
                           null,
                           $"User cannot be found for email: '{userEmail}'.");

            }

            if (!await _userMgr.IsEmailConfirmedAsync(user))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                           "User email address is not confirmed. Please confirm your email address and try again later.",
                           Errors.ErrorEmailNotConfirmed,
                           null,
                           $"ResetPasswordUrl cannot be generated, user email in not confirmed: '{user.Email}'.");
            }

            var token = await _userMgr.GeneratePasswordResetTokenAsync(user);

            var backendResetPasswordEmailUrl = Url.Link("ResetPasswordRoute", new { userId = user.Id, token = token });

            var resetPasswordEmailUrl = $"{_config["FrontApp:Domain"]}{_config["FrontApp:ResetPasswordUrl"]}{QueryString.Create("reseturl", backendResetPasswordEmailUrl).ToString()}";

            return resetPasswordEmailUrl;

        }

    }
}
