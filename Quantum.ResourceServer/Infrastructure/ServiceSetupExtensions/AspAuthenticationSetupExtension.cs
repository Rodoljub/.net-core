using Duende.IdentityServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Quantum.ResourceServer.Infrastructure.ServiceSetupExtensions
{
    public static class AspAuthenticationSetupExtension
    {
        public static IServiceCollection ConfigureAspAuthentication(this IServiceCollection services, IConfiguration _config)
        {
            var authBuilder = services.AddAuthentication();

            authBuilder.AddGoogle("Google", options =>
                    {
                        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        //options.CorrelationCookie.IsEssential = true;
                        //options.CorrelationCookie.SameSite = SameSiteMode.Strict;
                        options.ClientId = _config.GetSection("Social:Google:AppKey").Value;
                        options.ClientSecret = _config.GetSection("Social:Google:Secret").Value;

                    });

            authBuilder.AddFacebook("Facebook", options =>
               {
                   options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                   //options.CorrelationCookie.IsEssential = true;
                   //options.CorrelationCookie.SameSite = SameSiteMode.Strict;
                   options.ClientId = _config.GetSection("Social:Facebook:AppKey").Value;
                   options.ClientSecret = _config.GetSection("Social:Facebook:Secret").Value;
               });

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;

                    return Task.CompletedTask;
                };
            });

            return services;
        }
    }
}
