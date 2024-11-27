using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data
{
    public static class ServiceCollectionExtension
    {
        public static IIdentityServerBuilder AddIdentityDataStorage(this IIdentityServerBuilder identityBuilder, IConfiguration _config)
        {
            identityBuilder.AddConfigurationStore(configStoreOptions =>
            {
                configStoreOptions.ConfigureDbContext = builder =>
                                builder.UseSqlServer(_config.GetSection("Data:connectionString").Value, b => b.MigrationsAssembly("Quantum.Data.Identity.Migrations"));
            })

         // this adds the operational data from DB (codes, tokens, consents)
         .AddOperationalStore(options =>
         {
             options.ConfigureDbContext = builder =>
             builder.UseSqlServer(_config.GetSection("Data:connectionString").Value, b => b.MigrationsAssembly("Quantum.Data.Identity.Migrations"));

             // this enables automatic token cleanup. this is optional.
             options.EnableTokenCleanup = true;
             options.TokenCleanupInterval = _config.GetSection("OperationalStore:TokenCleanupInterval").Get<int>(); // interval in seconds (default is 3600)
         })
         .AddConfigurationStoreCache();
         
            return identityBuilder;
        }
    }
}
