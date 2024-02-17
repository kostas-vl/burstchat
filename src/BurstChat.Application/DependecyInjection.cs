using BurstChat.Application.Services.BCryptService;
using BurstChat.Application.Services.ChannelsService;
using BurstChat.Application.Services.PrivateGroupsService;
using BurstChat.Application.Services.DirectMessagingService;
using BurstChat.Application.Services.ModelValidationService;
using BurstChat.Application.Services.ServersService;
using BurstChat.Application.Services.UserService;
using Microsoft.Extensions.DependencyInjection;

namespace BurstChat.Application
{
    /// <summary>
    /// A static class that contains various extension methods for helping make dependency injection
    /// be configured with less code.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// This method configures all application level services to the dependency injection system.
        /// <summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IBCryptService, BCryptProvider>();

            services
                .AddScoped<IChannelsService, ChannelsProvider>()
                .AddScoped<IPrivateGroupsService, PrivateGroupsProvider>()
                .AddScoped<IDirectMessagingService, DirectMessagingProvider>()
                .AddScoped<IServersService, ServersProvider>()
                .AddScoped<IModelValidationService, ModelValidationProvider>()
                .AddScoped<IUserService, UserProvider>();

            return services;
        }
    }
}
