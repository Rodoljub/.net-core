using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Quantum.Core;
using Quantum.Core.SignalR.Hubs;
using Quantum.ResourceServer.Infrastructure.Configurations;
using Quantum.ResourceServer.Infrastructure.HostBuilderSetupExtensions;
using Quantum.ResourceServer.Infrastructure.ServiceSetupExtensions;
using Quantum.Utility.Infrastructure.Middleware;

        await Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            //config.Sources.Clear();

            var env = hostingContext.HostingEnvironment;

            config
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            //config.AddEnvironmentVariables();
        })

        .ConfigureSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureServices((webHostContext, services) =>
            {
                services.AddSingleton(webHostContext.Configuration);

                services.AddSingleton(webHostContext.Configuration);

                services.ConfigureAspIdentity(webHostContext.Configuration);

                services.ConfigureIdentityServer(webHostContext.Configuration);

                services.ConfigureAspAuthentication(webHostContext.Configuration);

                services.ConfigureCors(webHostContext.Configuration);

                services.AddSignalR();

                services.AddMemoryCache();
                services.AddResponseCaching();

                services.AddControllers()
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

                services.AddHttpContextAccessor();

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                services.AddRecaptcha(new RecaptchaOptions
                {
                    SiteKey = webHostContext.Configuration.GetSection("Recaptcha:SiteKey").Value,
                    SecretKey = webHostContext.Configuration.GetSection("Recaptcha:SecretKey").Value
                });

                services.ConfigureSwagger(webHostContext.Configuration);

                services.AddServiceQuantumCore(webHostContext.Configuration);

                services.AddControllersWithViews();
            })
        .Configure((webHostContext, app) =>
            {
                var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (enviroment == "Development")
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantum API");
                        options.OAuthClientId("angular-client");
                        options.OAuthAppName(IdentityServerConstants.LocalApi.ScopeName);
                        options.OAuthUsePkce();
                    });
                }

                app.UseHttpStatusCodeExceptionMiddleware(enviroment == "Development");

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseCors(CorsSetupExtension.GeneralPolicy);

                app.UseIdentityServer();
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseResponseCaching();

                app.UseEndpoints(endpoints =>
                {
                    //endpoints.MapGraphQL();
                    endpoints.MapDefaultControllerRoute();

                    endpoints.MapControllers();

                    endpoints.MapHub<EventsHub>("/events");
                });
            });

    }).RunConsoleAsync();


