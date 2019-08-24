﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.Database.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile("appsettings.Domains.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile("appsettings.IdentitySecrets.json", optional: true, reloadOnChange: false);
                    config.AddJsonFile("appsettings.SigningCredentials.json", optional: false, reloadOnChange: false);
                })
                .UseKestrel()
                .UseStartup<Startup>();
    }
}
