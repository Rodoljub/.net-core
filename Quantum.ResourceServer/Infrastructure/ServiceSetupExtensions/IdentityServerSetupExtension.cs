using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Data;
using System;

namespace Quantum.ResourceServer.Infrastructure.ServiceSetupExtensions
{
    public static class IdentityServerSetupExtension
    {
        public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services, IConfiguration _config)
        {
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.Discovery.CustomEntries.Add("local_api", "~/api");

                options.Authentication.CookieLifetime = TimeSpan.FromHours(30);
                options.Authentication.CookieSlidingExpiration = true;

                //options.Authentication.CookieSameSiteMode = SameSiteMode.Strict;

                //options.KeyManagement.Enabled = true;

                //options.KeyManagement.RotationInterval = TimeSpan.FromDays(30);

                //// announce new key 2 days in advance in discovery
                //options.KeyManagement.PropagationTime = TimeSpan.FromDays(2);

                //// keep old key for 7 days in discovery for validation of tokens
                //options.KeyManagement.RetentionDuration = TimeSpan.FromDays(7);

                //// don't delete keys after their retention period is over
                //options.KeyManagement.DeleteRetiredKeys = false;
            })
            .AddIdentityDataStorage(_config)
            .AddAspNetIdentity<IdentityUser>()//;
            //.AddSigningCredential(cert)
            .AddDeveloperSigningCredential(); // TODO: add prod. cert.

            builder.Services.ConfigureApplicationCookie(options =>
            {
                //options.Cookie.Expiration
                options.Cookie.Name = "IdentityServer.Coockie";

                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Strict;

            });

            services.AddLocalApiAuthentication();

            services.AddScoped<SecurityHeadersAttribute>();

            return services;
        }
    }
}
