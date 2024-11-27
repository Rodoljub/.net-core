using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Data;
using System;

namespace Quantum.ResourceServer.Infrastructure.ServiceSetupExtensions
{
    public static class AspIdentitySetupExtension
    {
        public static IServiceCollection ConfigureAspIdentity(this IServiceCollection services, IConfiguration _config)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<QDbContext>()
             .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = _config.GetSection("User:Password:RequiredLength").Get<int>();
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });

            return services;
        }
    }
}
