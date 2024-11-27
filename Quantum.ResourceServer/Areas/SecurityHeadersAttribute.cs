// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace IdentityServerHost.Quickstart.UI
{
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        private readonly IConfiguration _config;

        public SecurityHeadersAttribute(IConfiguration config)
        {
            _config = config;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
               var headers =  _config.GetSection("IdentityPages:SecurityHeaders").GetChildren()
                  .ToDictionary(x => x.Key, x => x.Value);

                foreach (var header in headers)
                {
                    if (!context.HttpContext.Response.Headers.ContainsKey(header.Key))
                    {
                        context.HttpContext.Response.Headers.Add(header.Key, header.Value);
                    }
                }


                //// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
                //if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options") /*&& !context.HttpContext.Request.Path.Value.Contains("Account/Login")*/)
                //{
                //    context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                //}

                //// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                //if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                //{
                //    context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                //}

                //// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
                ////var csp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";
                //var csp = "default-src 'self'; object-src 'none'; frame-ancestors https://www.occpy.com; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self'; script-src 'sha256-ZdHxw9eWtnxUb3mk6tBS+gIiVUPE3pGM470keHPDFlE=' 'sha256-JoRAFjcrOzrQhuFa0V82rwlCxGUvsnK6KpmJyaKsJ4E='";

                //// also consider adding upgrade-insecure-requests once you have HTTPS in place for production
                ////csp += "upgrade-insecure-requests;";
                //// also an example if you need client images to be displayed from twitter
                //// csp += "img-src 'self' https://pbs.twimg.com;";

                //// once for standards compliant browsers
                //if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                //{
                //    context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
                //}
                //// and once again for IE
                //if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                //{
                //    context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
                //}

                //// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                //var referrer_policy = "no-referrer";
                //if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
                //{
                //    context.HttpContext.Response.Headers.Add("Referrer-Policy", referrer_policy);
                //}
            }
        }
    }
}
