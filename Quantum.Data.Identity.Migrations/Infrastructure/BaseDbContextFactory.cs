using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Data;
using Serilog;
using System;
using System.IO;

namespace Quantum.Data.Identity.Migrations.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseDbContextFactory<T> : IDesignTimeDbContextFactory<T>
        where T : DbContext
    {

        public BaseDbContextFactory()
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            Log.Information($"{nameof(BaseDbContextFactory<T>)} Initialized");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public T CreateDbContext(string[] args)
        {
            return Create();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public virtual T Create()
        {
            Log.Information($"Getting ASPNETCORE_ENVIRONMENT info...");

            //var envConfig = new ConfigurationBuilder()
            //    .AddEnvironmentVariables()
            //    .Build();

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //var environmentName = envConfig.GetSection("ASPNETCORE_ENVIRONMENT").Value;

            Log.Information($"Environment name: {environmentName}");

            var basePath = FindSettingsFolder(Directory.GetCurrentDirectory(), environmentName);  //AppContext.BaseDirectory;

            return Create(basePath, environmentName);
        }

        private static T Create(string basePath, string environmentName)
        {
            Log.Information($"Buildeing configuration");

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            return Create(config);

        }

        private static T Create(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentException($"{nameof(config)} is null or empty.", nameof(config));
            }

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString) == true)
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
            }

            Log.Information($"Setting context options");

            var migrationsAssembly = typeof(BaseDbContextFactory<>).Assembly.GetName().Name;

            var services = new ServiceCollection();

            services.AddDbContext<QDbContext>(options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Quantum.Data.Migrations"));

                options.EnableSensitiveDataLogging();
            });

            //services.AddEntityFrameworkNpgsql();

            services.AddIdentityCore<IdentityUser>()
           .AddEntityFrameworkStores<QDbContext>();

            var builder = services.AddIdentityServer();

            builder.AddAspNetIdentity<IdentityUser>();

            // this adds the config data from DB (clients, resources, CORS)
            builder.AddConfigurationStore(configStoreOptions =>
            {
                configStoreOptions.ConfigureDbContext = confBuilder =>
                    confBuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(ConfigurationDbContextFactory).Assembly.GetName().Name));
            });

            // this adds the operational data from DB (codes, tokens, consents)
            builder.AddOperationalStore(configStopeOptions =>
            {
                configStopeOptions.ConfigureDbContext = confBuilder =>
                    confBuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(PersistedGrantDbContextFactory).Assembly.GetName().Name));

                // this enables automatic token cleanup. this is optional.
                configStopeOptions.EnableTokenCleanup = true; //config.GetSection("OperationalStore:EnableTokenCleanup").Get<bool>();
                //configStopeOptions.TokenCleanupInterval = config.GetSection("OperationalStore:TokenCleanupInterval").Get<int>();
            });

            var serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();


            var context = scope.ServiceProvider.GetService<T>();

            if (context != null && context.Database != null)
            {
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Migrate error: {ex}");
                }

                if (typeof(T) == typeof(ConfigurationDbContext))
                {
                    if (context is ConfigurationDbContext confDbContext)
                        SeedConfiguration(config, confDbContext);
                }
            }

            return context;
        }

        private static void SeedConfiguration(IConfiguration config, ConfigurationDbContext context)
        {
            Log.Information($"Creating ConfigurationDbContext instance");

            var seeder = new ConfigurationDbSeeder(config, context);

            //seeder.Seed();
        }

        private string FindSettingsFolder(string basePath, string environmentName)
        {
            var appsettingsFileName = !string.IsNullOrWhiteSpace(environmentName) ? $"appsettings.{environmentName}.json" : "appsettings.json";

            if (!File.Exists(Path.Combine(basePath, appsettingsFileName)))
            {
                var parentFolder = Directory.GetParent(basePath);

                if (parentFolder != null && parentFolder.Exists)
                {
                    basePath = parentFolder.FullName;

                    return FindSettingsFolder(basePath, environmentName);
                }
                else
                {
                    throw new Exception($"{appsettingsFileName} file cannot be found.");
                }
            }
            else
            {
                return basePath;
            }
        }
    }
}
