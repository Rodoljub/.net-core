using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System;
using SixLabors.ImageSharp;
using Swashbuckle.AspNetCore.SwaggerGen;
using Duende.IdentityServer;

namespace Quantum.ResourceServer.Infrastructure.ServiceSetupExtensions
{
    public static class SwaggerSetupExtension
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration config)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Quantum API", Version = "v1" });
                options.EnableAnnotations();

                // Set the comments path for the Swagger JSON and UI.
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"connect/authorize", UriKind.Relative),
                            TokenUrl = new Uri($"connect/token", UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                                [IdentityServerConstants.LocalApi.ScopeName] = IdentityServerConstants.LocalApi.ScopeName
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeOperationFilter>();
            });

            return services;
        }
    }

    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Since all the operations in our api are protected, we need not
            // check separately if the operation has Authorize attribute
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}
                    }
                ] = new[] { IdentityServerConstants.LocalApi.ScopeName }
            }
        };
        }
    }
}
