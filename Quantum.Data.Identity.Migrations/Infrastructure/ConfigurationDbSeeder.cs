using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Data.Identity.Migrations.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigurationDbSeeder
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IConfiguration _config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// /// <param name="config"></param>
        public ConfigurationDbSeeder(IConfiguration config, ConfigurationDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;


        }

        /// <summary>
        /// Seeds this instance.
        /// </summary>
        public Task<int> Seed()
        {

            _dbContext.Database.Migrate();

            Log.Information($"Starting IdentityServr Configuration seeding...");

            AddClientsFromConfiguration();

            AddIdentityResources();

            AddApiResourcesFromConfiguration();

            AddApiScopesFromConfiguration();

            Log.Information($"Seeding completed.");

            return _dbContext.SaveChangesAsync();

        }

        private void AddApiResourcesFromConfiguration()
        {
            try { _dbContext.ApiResources.Any(); }
            catch
            {
                Log.Information($"Entity ApiResources does not exist.");
                return;
            }

            var apiResources = Config.Apis.ToArray(); // _config.GetSection("IdentityServer:ApiResources").Get<ApiResource[]>();

            if (apiResources != null && apiResources.Length > 0 && apiResources[0] != null)
            {
                var existingResources = _dbContext.ApiResources.Where(res => apiResources.Select(nres => nres.Name).Contains(res.Name))
                    .Select(res => res.Name).ToList();

                var onlyNewApiResources = apiResources.Where(nres => !existingResources.Contains(nres.Name));

                if (onlyNewApiResources.Any())
                {
                    foreach (var apirResource in onlyNewApiResources)
                    {
                        _dbContext.ApiResources.Add(apirResource.ToEntity());
                    }
                    _dbContext.SaveChanges();

                    var count = onlyNewApiResources.Count();

                   Log.Information($"{count} new ApiResource{(count > 1 ? "s were" : " was")} successfully created.");
                }
                else
                {
                   Log.Information($"ApiResources are already up to date.");
                }
            }
            else
            {
               Log.Information($"ApiResource list is empty. ApiResources were not updated.");
            }
        }


        private void AddApiScopesFromConfiguration()
        {
            try { _dbContext.ApiScopes.Any(); }
            catch
            {
                Log.Information($"Entity ApiScopes does not exist.");
                return;
            }

            var apiScopes = Config.Scopes.ToArray();//_config.GetSection("IdentityServer:ApiScopes").Get<ApiScope[]>();

            if (apiScopes != null && apiScopes.Length > 0 && apiScopes[0] != null)
            {
                var existingScopes = _dbContext.ApiScopes.Where(res => apiScopes.Select(nres => nres.Name).Contains(res.Name))
                    .Select(res => res.Name).ToList();

                var onlyNewApiScopes = apiScopes.Where(nres => !existingScopes.Contains(nres.Name));

                if (onlyNewApiScopes.Any())
                {
                    foreach (var apirResource in onlyNewApiScopes)
                    {
                        _dbContext.ApiScopes.Add(apirResource.ToEntity());
                    }
                    _dbContext.SaveChanges();

                    var count = onlyNewApiScopes.Count();

                    Log.Information($"{count} new ApiScope{(count > 1 ? "s were" : " was")} successfully created.");
                }
                else
                {
                   Log.Information($"ApiScopes are already up to date.");
                }
            }
            else
            {
               Log.Information($"ApiResource list is empty. ApiScopes were not updated.");
            }
        }

        private void AddIdentityResources()
        {
            try { _dbContext.IdentityResources.Any(); }
            catch
            {
               Log.Information($"Entity IdentityResources does not exist.");
                return;
            }

            //var apparatusProfile = _config.GetSection("IdentityServer:IdentityResource").Get<IdentityResource>();

            var resources = new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                //apparatusmProfile
            };

            if (resources != null && resources.Length > 0 && resources[0] != null)
            {
                var existingResources = _dbContext.IdentityResources.Where(res => resources.Select(nres => nres.Name).Contains(res.Name))
                    .Select(res => res.Name).ToList();

                var onlyNewResources = resources.Where(nres => !existingResources.Contains(nres.Name));

                if (onlyNewResources.Any())
                {
                    foreach (var resource in onlyNewResources)
                    {
                        _dbContext.IdentityResources.Add(resource.ToEntity());
                    }
                    _dbContext.SaveChanges();

                    var count = onlyNewResources.Count();

                    Log.Information($"{count} new Resource{(count > 1 ? "s were" : " was")} successfully created.");
                }
                else
                {
                    Log.Information($"Resources are already up to date.");
                }
            }
            else
            {
                Log.Information($"Resource list is empty. Resources were not updated.");
            }
        }

        private void AddClientsFromConfiguration()
        {
            try { _dbContext.Clients.Any(); }
            catch
            {
               Log.Information($"Entity Clients does not exist.");
                return;
            }

            var clients = Config.Clients.ToArray();// _config.GetSection("IdentityServer:Clients").Get<Client[]>();

            if (clients != null && clients.Length > 0 && clients[0] != null)
            {
                Log.Information($"Starting client seeding");

                var existingClients = _dbContext.Clients
                    .Where(c => clients.Select(ncli => ncli.ClientId).Contains(c.ClientId))
                    .Select(c => c.ClientId).ToList();

                var onlyNewClients = clients.Where(ncli => !existingClients.Contains(ncli.ClientId));

                if (onlyNewClients.Any())
                {
                    foreach (var client in onlyNewClients)
                    {
                        _dbContext.Clients.Add(client.ToEntity());
                    }

                    _dbContext.SaveChanges();

                    var count = onlyNewClients.Count();

                    Log.Information($"{count} new client{(count > 1 ? "s were" : " was")} successfully created.");
                }
                else
                {
                   Log.Information($"Clients are already up to date.");
                }
            }
            else
            {
               Log.Information($"Client list is empty. Clients were not updated.");
            }
        }
    }
}
