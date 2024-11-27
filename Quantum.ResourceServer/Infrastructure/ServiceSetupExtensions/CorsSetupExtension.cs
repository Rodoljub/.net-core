using Duende.IdentityServer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quantum.ResourceServer.Infrastructure.Configurations;

namespace Quantum.ResourceServer.Infrastructure.ServiceSetupExtensions
{
    public static class CorsSetupExtension
    {
        public const string GeneralPolicy = "GeneralPolicy";

        public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration _config)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: GeneralPolicy,
                    builder =>
                    {
                        var corsOptions = _config.GetSection("CORS").Get<CorsConfiguration>() ?? new CorsConfiguration(loadDafault: true);

                        builder.AllowAnyHeader()
                        .WithOrigins(corsOptions.AllowedOrigins)
                        .WithMethods(corsOptions.AllowedMethods)
                        .AllowCredentials(); 
                    });
            });

            //services.AddSingleton<ICorsPolicyService>((container) =>
            //{
            //    var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
            //    return new DefaultCorsPolicyService(logger)
            //    {
            //        AllowedOrigins = { _config.GetSection("CORS:AllowedOrigins").Value }
            //    };
            //});

            return services;
        }
    }
}
