using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Servers;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat
{
    public partial class ChatHub
    {
        /// <summary>
        /// Constructs the appropriate signal server name of all users of a server.
        /// </summary>
        /// <param name="id">The id of the server</param>
        /// <returns>The string signal server name</returns>
        private string ServerSignalName(int id) => $"server:{id}";

        /// <summary>
        /// Adds a new server and informs the caller of the results.
        /// </summary>
        /// <param name="server">The server instance to be added</param>
        /// <returns>A Task instance</returns>
        public async Task AddServer(Server server)
        {
            var httpContext = Context.GetHttpContext();
            var sub = Context;
            var monad = await _serverService.PostAsync(httpContext, server);

            switch (monad)
            {
                case Success<Server, Error> success:
                    await Clients.Caller.AddedServer(success.Value);
                    break;

                case Failure<Server, Error> failure:
                    await Clients.Caller.AddedServer(failure.Value);
                    break;

                default:
                    await Clients.Caller.AddedServer(SystemErrors.Exception());
                    break;
            }
        }

        /// <summary>
        /// Adds a new connection to a signalr group that is based on the id of a BurstChat server.
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>A Task instance</returns>
        public async Task AddToServer(int serverId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _serverService.GetAsync(httpContext, serverId);

            if (monad is Success<Server, Error>)
            {
                var signalGroup = ServerSignalName(serverId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
            }
        }
    }
}