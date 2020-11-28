using System;
using System.Net;
using BurstChat.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BurstChat.Signal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureAppConfiguration(config =>
                        {
                            config.AddJsonFile("appsettings.AcceptedDomains.json", optional: false, reloadOnChange: false);
                            config.AddJsonFile("appsettings.AccessTokenValidation.json", optional: false, reloadOnChange: false);
                        })
                        .ConfigureLogging(logging =>
                        {
                            logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
                            logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
                        })
                        .UseKestrel(options =>
                        {
                            var envHost = Environment.GetEnvironmentVariable(EnvironmentVariables.BURST_CHAT_SIGNAL_HOST);
                            var envPort = Environment.GetEnvironmentVariable(EnvironmentVariables.BURST_CHAT_SIGNAL_PORT);

                            if (envHost != null && envPort != null)
                            {
                                var canParseHost = IPAddress.TryParse(envHost, out var host);
                                if (!canParseHost)
                                    throw new Exception($"{EnvironmentVariables.BURST_CHAT_SIGNAL_HOST} invalid value");

                                var canParsePort = Int32.TryParse(envPort, out var port);
                                if (!canParsePort)
                                    throw new Exception($"{EnvironmentVariables.BURST_CHAT_SIGNAL_PORT} invalid value");

                                options.Listen(host, port);
                            }
                            else
                                options.ListenLocalhost(5001);
                        })
                        .UseStartup<Startup>();
                });
    }
}
