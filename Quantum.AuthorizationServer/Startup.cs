using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Quantum.AuthorizationServer.Services;
using Quantum.AuthorizationServer.Services.Contracts;
using Quantum.Data;
using Quantum.Data.Entities;
using Quantum.Data.Repositories;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Common.Contracts;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Infrastructure.Authorization;
using Quantum.Utility.Infrastructure.Middleware;
using Quantum.Utility.Services;
using Quantum.Utility.Services.Contracts;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _env = env;
            _config = configuration.Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Async(x => x.RollingFile("Logs/AS-{Date}.txt",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"))
                //.ReadFrom.Configuration(_config)
                .CreateLogger();
        }

        public IConfigurationRoot _config { get; }
        private IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            services.AddSingleton(_config);

            //Repository configuration
            services.AddDbContext<QDbContext>(ServiceLifetime.Scoped);
            
            services.AddScoped<IFileTypeRepository, FileTypeRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISocialService, SocialService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IDocumentService, DocumentService>();
			services.AddScoped<IMappingFileService, MappingFileService>();

            services.AddScoped<IBaseRepository<Folder, IdentityUser>, BaseRepository<Folder>>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
           
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAutoMapper();
            services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<QDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = Convert.ToInt32(_config["User:Password:RequiredLength"]);
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;


                //options.Cookies.ApplicationCookie.AutomaticChallenge = false;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.Audience = _config["Tokens:Audience"];
                //options.Authority = _config["Tokens:Authority"];
                
                if (!environmentName.ToLower().StartsWith("prod"))
                {
                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                }

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = _config["Tokens:Issuer"],
                    ValidAudience = _config["Tokens:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"])),
                    ValidateLifetime = true,

                    //LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
                    //{
                    //	if(expires.HasValue)
                    //	{
                    //		return expires.Value.ToUniversalTime() > DateTime.UtcNow;
                    //	}

                    //	return false;
                    //}
                };
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;

                    return Task.CompletedTask;
                };
            });



            services.AddCors(cfg =>
            {
                cfg.AddPolicy("GeneralPolicy", bldr =>
                {
                    bldr.AllowAnyHeader()
                       //.AllowAnyMethod()
                       .WithMethods("GET", "POST", "PUT", "DELETE")
                       .AllowAnyOrigin();
                    //.WithOrigins(_config["CORS:AllowedOrigins"]);
                });
            });

            services.AddMemoryCache();

            // Add framework services.
            services.AddMvc(opt =>
                {
                    //opt.Filters.Add(new RequireHttpsAttribute());
                })
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = _config["Recaptcha:SiteKey"],
                SecretKey = _config["Recaptcha:SecretKey"]
            });

            services.AddAuthorization(options => { });

            

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            //services.AddMvcCore()
            //	.AddAuthorization() // Note - this is on the IMvcBuilder, not the service collection
            //	.AddJsonFormatters(options => options.ContractResolver = new CamelCasePropertyNamesContractResolver());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            //IMemoryCache cache,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            app.UseHttpStatusCodeExceptionMiddleware(env.IsDevelopment());

            app.UseCors(b => b.AllowAnyHeader()
                .WithOrigins(_config["CORS:AllowedOrigins"])
                .WithMethods("GET", "POST"/*, "PUT", "DELETE"*/)
            //.AllowAnyMethod()
            );

            app.UseAuthentication();

            app.UseMvc();

        }
    }
}
