using System;
using System.Net;
using BurstChat.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BurstChat.Api
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
                        .ConfigureAppConfiguration((context, config) =>
                        {
                            config.AddJsonFile("appsettings.Database.json", optional: false, reloadOnChange: false);
                            config.AddJsonFile("appsettings.Domains.json", optional: false, reloadOnChange: false);
                            config.AddJsonFile("appsettings.AccessTokenValidation.json", optional: false, reloadOnChange: false);
                        })
                        .UseKestrel(options =>
                        {
                            var envHost = Environment.GetEnvironmentVariable(EnvironmentVariables.BURST_CHAT_API_HOST);
                            var envPort = Environment.GetEnvironmentVariable(EnvironmentVariables.BURST_CHAT_API_PORT);

                            if (envHost != null && envPort != null)
                            {
                                var canParseHost = IPAddress.TryParse(envHost, out var host);
                                if (!canParseHost)
                                    throw new Exception($"{EnvironmentVariables.BURST_CHAT_API_HOST} invalid value");

                                var canParsePort = Int32.TryParse(envPort, out var port);
                                if (!canParsePort)
                                    throw new Exception($"{EnvironmentVariables.BURST_CHAT_API_PORT} invalid value");

                                options.Listen(host, port);
                            }
                            else
                                options.ListenLocalhost(5000);
                        })
                        .UseStartup<Startup>();
                });
    }
}
