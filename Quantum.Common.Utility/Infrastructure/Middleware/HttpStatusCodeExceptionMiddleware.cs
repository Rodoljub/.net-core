using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Quantum.Utility.Infrastructure.Middleware
{
	public class HttpStatusCodeExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpStatusCodeExceptionMiddleware> _logger;
        private bool isDevEnvironment = false;
		private IConfiguration _config;


		public HttpStatusCodeExceptionMiddleware(
			RequestDelegate next, 
			ILoggerFactory loggerFactory, 
			bool devEnv,
			IConfiguration config
		)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = loggerFactory?.CreateLogger<HttpStatusCodeExceptionMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            isDevEnvironment = devEnv;
			_config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;

                if (ex is HttpStatusCodeException httpEx && !string.IsNullOrWhiteSpace(httpEx.LogEntryPrefix))
                {
                    _logger.LogError(ex, $"{httpEx.LogEntryPrefix}: {httpEx.InnerException}");
                }
                else
                {
                    _logger.LogError(ex, ex.Message);
                }

                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.ContentType = @"application/json";
					context.Response.Headers.Add("Access-Control-Allow-Origin", _config["CORS:AllowedOrigins"]);

					var message = "Ooops! Something went wrong. Please try later.";
                    var lkey = Errors.GeneralError;

					//

                    if (ex is HttpStatusCodeException httpExep)
                    {
                        context.Response.StatusCode = (int)httpExep.StatusCode;
                        message = httpExep.UserMessage;
                        lkey = httpExep.LocalizationKey;
                    }

                    var error = JsonSerializer.Serialize(new
                    //JsonConvert.SerializeObject(new
                    {
                        Message = message,
                        Key = lkey,
                        DebugInfo = isDevEnvironment ? ex.Message : null
                    });

					var encodedError = Encoding.ASCII.GetBytes(error);

					await context.Response.Body.WriteAsync(encodedError, 0, encodedError.Length)
								 .ConfigureAwait(false);

				}
            }
        }
    }

    public static class HttpStatusCodeExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpStatusCodeExceptionMiddleware(this IApplicationBuilder builder, bool isDevelopment = false)
        {
            return builder.UseMiddleware<HttpStatusCodeExceptionMiddleware>(isDevelopment);
        }
    }
}
