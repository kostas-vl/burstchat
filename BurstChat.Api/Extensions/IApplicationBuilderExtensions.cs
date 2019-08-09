using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BurstChat.Api.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseBurstChatClients(this IApplicationBuilder application)
        {
            var serviceScopeFactory = application
                .ApplicationServices
                .GetService<IServiceScopeFactory>();

            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                serviceScope
                    .ServiceProvider
                    .GetRequiredService<PersistedGrantDbContext>()
                    .Database
                    .Migrate();

                var context = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ConfigurationDbContext>();

                context
                    .Database
                    .Migrate();

                var noClients = !context
                    .Clients
                    .Any();

                if (noClients)
                {
                    
                }
            }
        }
    }
}
