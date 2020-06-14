using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BurstChat.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BurstChat.IdentityServer
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
                            config.AddJsonFile("appsettings.Smtp.json", optional: false, reloadOnChange: false);
                            config.AddJsonFile("appsettings.IdentitySecrets.json", optional: true, reloadOnChange: false);
                            config.AddJsonFile("appsettings.SigningCredentials.json", optional: false, reloadOnChange: false);
                            config.AddJsonFile("appsettings.AlphaCodes.json", optional: false, reloadOnChange: false);
                        })
                        .UseKestrel(options =>
                        {
                            var envHost = Environment.GetEnvironmentVariable(EnvironmentVariables.BURST_CHAT_IDENTITY_HOST);
                            var envPort = Environment.GetEnvironmentVariable(EnvironmentVariables.BURST_CHAT_IDENTITY_PORT);

                            if (envHost != null && envPort != null)
                            {
                                var canParseHost = IPAddress.TryParse(envHost, out var host);
                                if (!canParseHost)
                                    throw new Exception($"{EnvironmentVariables.BURST_CHAT_IDENTITY_HOST} invalid value");

                                var canParsePort = Int32.TryParse(envPort, out var port);
                                if (!canParsePort)
                                    throw new Exception($"{EnvironmentVariables.BURST_CHAT_IDENTITY_PORT} invalid value");

                                options.Listen(host, port);
                            }
                            else
                                options.ListenLocalhost(5002);
                        })
                        .UseStartup<Startup>();
                });
    }
}