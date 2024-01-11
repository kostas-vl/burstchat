import { Component, effect, untracked } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faDragon, faUsers } from '@fortawesome/free-solid-svg-icons';
import { Server } from 'src/app/models/servers/server';
import { DisplayServer } from 'src/app/models/sidebar/display-server';
import { Channel } from 'src/app/models/servers/channel';
import { Invitation } from 'src/app/models/servers/invitation';
import { Subscription as BurstSubscription } from 'src/app/models/servers/subscription';
import { UserService } from 'src/app/services/user/user.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { ServersService } from 'src/app/services/servers/servers.service';
import { SidebarService } from 'src/app/services/sidebar/sidebar.service';
import { DirectMessagingComponent } from 'src/app/components/core/direct-messaging/direct-messaging.component';
import { ServerComponent } from 'src/app/components/core/server/server.component';

/**
 * This class represents an angular component that displays on screen a list of subscribed servers.
 * @export
 * @class SidebarSelectionComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-sidebar-selection',
    templateUrl: './sidebar-selection.component.html',
    styleUrl: './sidebar-selection.component.scss',
    standalone: true,
    imports: [FontAwesomeModule, DirectMessagingComponent, ServerComponent]
})
export class SidebarSelectionComponent {

    public dragon = faDragon;

    public users = faUsers;

    public servers = this.userService.subscriptions;

    /**
     * Creates an instance of SidebarSelectionComponent.
     * @memberof SidebarSelectionComponent
     */
    constructor(
        private userService: UserService,
        private serversService: ServersService,
        private chatService: ChatService,
        private sidebarService: SidebarService
    ) {
        effect(() => this.serverInfoCallback(this.chatService.addedServer()), { allowSignalWrites: true });
        effect(() => this.updatedServer(this.chatService.updatedServer()), { allowSignalWrites: true });
        effect(() => this.subcriptionDeletedCallback(this.chatService.subscriptionDeleted()), { allowSignalWrites: true });
        effect(() => this.channelCreatedCallback(this.chatService.channelCreated()), { allowSignalWrites: true });
        effect(() => this.channelUpdatedCallback(this.chatService.channelUpdated()), { allowSignalWrites: true });
        effect(() => this.channelDeletedCallback(this.chatService.channelDeleted()), { allowSignalWrites: true });
        effect(() => this.updatedInvitationCallback(this.chatService.updatedInvitation()), { allowSignalWrites: true });
        effect(() => this.serverInfoCallback(this.serversService.serverInfo()), { allowSignalWrites: true });

        effect(() => {
            const options = this.sidebarService.display();
            if (options instanceof DisplayServer && options.serverId) {
                untracked(() => this.serversService.set(options.serverId));
            }
        });

        effect(() => {
            const servers = this.userService.subscriptions();
            this.serversService.updateCache(servers);
        }, { allowSignalWrites: true });
    }

    /**
     * This methos is invoked when a server's information is pushed by the server info
     * observable.
     * @private
     * @param {Server | null} server The server instance.
     * @memberof SidebarSelectionComponent
     */
    private serverInfoCallback(server: Server | null) {
        if (server) {
            const servers = this.userService.subscriptions();
            const index = servers.findIndex(s => s.id === server.id);
            if (index !== -1) {
                this.userService.updateSubscriptions(server, index);
                this.servers[index] = server;
            } else {
                this.userService.updateSubscriptions(server);
            }
        }
    }

    /**
     * Updates the info of the provided server both the cached entry and the current selected
     * server
     * @param {Server} server The updated server instance.
     * @memberof SidebarSelectionComponent
     */
    private updatedServer(server: Server) {
        if (server) {
            this.serverInfoCallback(server);
            const current = this.serversService.current();
            if (server.id === current.id) {
                this.serversService.set(server.id);
            }
        }
    }

    /**
     * This method is invoked when a new value is sent from the subscription deleted
     * observale.
     * @private
     * @param {[number, BurstSubscription]} data The server id and the subscription instance.
     * @memberof SidebarSelectionComponent
     */
    private subcriptionDeletedCallback(data: [number, BurstSubscription] | null) {
        if (!data) return;
        const serverId = data[0];
        const subscription = data[1];
        const servers = this.servers();
        const server = servers.find(s => s.id === serverId);
        if (server && subscription) {
            const index = server
                .subscriptions
                .findIndex(s => s.userId === subscription.userId);
            if (index !== -1) {
                server.subscriptions.splice(index, 1);
                this.serversService.updateCache(servers);
            }
            const users = this.userService.usersCache()[server.id] || [];
            const usersIndex = users.findIndex(u => u.id === subscription.userId);
            if (usersIndex !== -1) {
                users.splice(usersIndex, 1);
                this.userService.pushToCache(server.id, users);
            }
        }
    }

    /**
     * This method is invoked when a new channel is pushed by the channel created observable.
     * @private
     * @param {[number, Channel] | null} data The server id and the channel instance
     * @memberof SidebarSelectionComponent
     */
    private channelCreatedCallback(data: [number, Channel] | null) {
        if (!data) return;
        const serverId = data[0];
        const channel = data[1];
        const servers = this.servers();
        const server = servers.find(s => s.id === serverId);
        if (server && server.channels && server.channels.length > 0) {
            server.channels.push(channel);
            this.serversService.updateCache(servers);
        }
    }

    /**
     * This method is invoked when the updated information about a channel is pushed
     * by the channel updated observable.
     * @private
     * @param {Channel} channel The updated channel instance.
     * @memberof SidebarSelectionComponent
     */
    private channelUpdatedCallback(channel: Channel | null) {
        if (!channel) return;

        const servers = this.servers();
        const server = servers.find(s => s.channels.some(c => c.id === channel.id));
        if (server) {
            const index = server.channels.findIndex(c => c.id === channel.id);
            if (index !== -1) {
                server.channels[index] = channel;
                this.serversService.updateCache(servers);
            }
        }
    }

    /**
     * This method is invoked when a channel deletion is pushed by the channel deleted
     * observable.
     * @private
     * @param {number | null} channelId The removed channel id.
     * @memberof SidebarSelectionComponent
     */
    private channelDeletedCallback(channelId: number | null) {
        const servers = this.userService.subscriptions();
        const server = servers?.find(s => s.channels.some(c => c.id === channelId));
        if (server) {
            const index = server.channels.findIndex(c => c.id === channelId);
            if (index !== -1) {
                server.channels.splice(index, 1);
                this.serversService.updateCache(servers);
            }
        }
    }

    /**
     * This method is invoked when an invitation is updated and pushed by the updated invitation
     * observable.
     * @private
     * @param {Invitation | null} invite The invitation instance.
     * @memberof SidebarSelectionComponent
     */
    private updatedInvitationCallback(invite: Invitation | null) {
        if (!invite) return;

        const servers = this.userService.subscriptions();
        const inList = servers.some(s => s.id === invite.serverId);

        // Handle code for the user the initiated the invitation update.
        const user = this.userService.user();
        if (invite.userId === user?.id && invite.accepted && !inList) {
            const server = invite.server;
            this.serversService.get(server.id);
            return;
        }

        // Handle code for all users of the server if the invitation was accepted.
        if (inList && invite.accepted) {
            const serverId = invite.serverId.toString();
            const server = servers.find(s => s.id === invite.serverId);
            const users = this.userService.usersCache()[serverId];

            users.push(invite.user);
            server.subscriptions.push({ userId: invite.userId, serverId: invite.serverId });

            this.serversService.updateCache(servers);
            this.userService.pushToCache(server.id, users);
            return;
        }
    }

    /**
     * Handles the new server button click event.
     * @memberof SidebarSelectionComponent
     */
    public onNew() {
        this.sidebarService.toggleAddServerDialog(true);
    }

    /**
      * A custom track method for the template loop over the server list.
      * @memberof SidebarSelectionComponent
      */
    public trackServerBy(server: Server) {
        return server.id;
    }

}

