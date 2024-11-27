using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Quantum.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<QDbContext>
    {

        public QDbContext Create()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var basePath = FindSettingsFolder(Directory.GetCurrentDirectory(), environmentName);  //AppContext.BaseDirectory;

            return Create(basePath, environmentName);
        }

        public QDbContext CreateDbContext(string[] args)
        {
            return Create();
        }

        private QDbContext Create(string basePath, string environmentName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var connstr = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connstr) == true)
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
            }
            else
            {
                return Create(connstr);
            }
        }

        private QDbContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));
            }

            var optionsBuilder = new DbContextOptionsBuilder<QDbContext>();
            optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("Quantum.Data.Migrations"));
            return new QDbContext(optionsBuilder.Options);
        }

        private string FindSettingsFolder(string basePath, string environmentName)
        {
            var appsettingsFileName = !string.IsNullOrWhiteSpace(environmentName) ? $"appsettings.{environmentName}.json" : "appsettings.json";

            if (!System.IO.File.Exists(Path.Combine(basePath, appsettingsFileName)))
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




