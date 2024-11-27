using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Quantum.ResourceServer.Infrastructure.HostBuilderSetupExtensions
{
    public static class SerilogSetupExtensions
    {
        public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((hostingContext, loggerConfig) =>
            {
                loggerConfig.Enrich.FromLogContext();
                loggerConfig.WriteTo.Console(LogEventLevel.Warning);
                loggerConfig.WriteTo.Async(asyncConf =>
                {
                    asyncConf.RollingFile(pathFormat: "Logs//{Date}.txt",
                        LogEventLevel.Warning,
                        //fileSizeLimitBytes: 10240,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}");
                });
                loggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
            });

            return hostBuilder;
        }
    }
}
