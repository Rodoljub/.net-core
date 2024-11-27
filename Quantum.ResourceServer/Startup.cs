using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Quantum.Core;
using Quantum.Core.SignalR.Hubs;
using Quantum.Data;
using Quantum.ResourceServer.Infrastructure.ServiceSetupExtensions;
using Quantum.Utility.Infrastructure.Middleware;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Quantum.ResourceServer
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _env = env;
            _config = configuration.Build(); ;


            Log.Logger = new LoggerConfiguration()
                .WriteTo.Async(x => x.RollingFile("Logs/RS-{Date}.txt",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"))
                //.ReadFrom.Configuration(_config)
                .CreateLogger();
        }

        public IConfiguration _config { get; }
        private IHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSingleton(_config);

            services.ConfigureAspIdentity(_config);

            services.ConfigureIdentityServer(_config);

            services.ConfigureAspAuthentication(_config);

            services.ConfigureCors(_config);

            services.AddSignalR();

            //var domain = _config["Application:Domain"];

            services.AddMemoryCache();
            services.AddResponseCaching();

            services.AddControllers()
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                //opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddHttpContextAccessor();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = _config["Recaptcha:SiteKey"],
                SecretKey = _config["Recaptcha:SecretKey"]
            });

            services.ConfigureSwagger(_config);

            services.AddServiceQuantumCore(_config);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostEnvironment env,
            ILoggerFactory loggerFactory)
        {

            if(env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantum API");
                });
            }

            app.UseHttpStatusCodeExceptionMiddleware(env.IsDevelopment());

            loggerFactory.AddSerilog();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("GeneralPolicy");

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                // For GetTypedHeaders, add: using Microsoft.AspNetCore.Http;
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        //MaxAge = TimeSpan.FromSeconds(10)
                    };
                //context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                //    new string[] { "Accept-Encoding" };

                context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors " + _config.GetSection("CORS:AllowedOrigins").Value);
                context.Response.Headers.Add("X-Frame-Options", $"ALLOW-FROM {_config.GetSection("CORS:AllowedOrigins").Value}");

                await next();
            });

            app.UseResponseCaching();

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGraphQL();
                endpoints.MapDefaultControllerRoute();

                endpoints.MapControllers();

                endpoints.MapHub<EventsHub>("/events");
            });

        }


    }
}
