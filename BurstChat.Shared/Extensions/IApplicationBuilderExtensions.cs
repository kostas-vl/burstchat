using System;
using System.Collections.Generic;
using System.Linq;
using BurstChat.Shared.Context;
using BurstChat.Shared.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BurstChat.Shared.Extensions
{
    /// <summary>
    /// This class exposes extension methods for the IApplicationBuilder interface.
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Register alpha invitation codes based on the configuration action provided.
        /// </summary>
        /// <param name="application">The application builder instance</param>
        /// <param name="alphaInvitationsCallback">The invitations configuration callback</param>
        public static void UseAlphaInvitationCodes(this IApplicationBuilder application, Action<AlphaInvitationCodesOptions> alphaInvitationsCallback)
        {
            var options = new AlphaInvitationCodesOptions();
            alphaInvitationsCallback(options);

            if (options?.AlphaCodes is {})
            {
                var serviceScopeFactory = application
                    .ApplicationServices
                    .GetService<IServiceScopeFactory>();

                using var serviceScope = serviceScopeFactory.CreateScope();

                var context = serviceScope
                    .ServiceProvider
                    .GetRequiredService<BurstChatContext>();

                var alphaInvitationCodes = context
                    .AlphaInvitations
                    .ToList();

                foreach (var code in alphaInvitationCodes)
                {
                    context.AlphaInvitations.Remove(code);
                }

                context.AlphaInvitations.AddRange(options.AlphaCodes);

                context.SaveChanges();
            }
        }
    }
}
