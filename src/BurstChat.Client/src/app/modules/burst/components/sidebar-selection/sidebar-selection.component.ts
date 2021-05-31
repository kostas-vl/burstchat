import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { faDragon, faUsers } from '@fortawesome/free-solid-svg-icons';
import { User } from 'src/app/models/user/user';
import { Server } from 'src/app/models/servers/server';
import { DisplayServer } from 'src/app/models/sidebar/display-server';
import { Channel } from 'src/app/models/servers/channel';
import { Invitation } from 'src/app/models/servers/invitation';
import { Subscription as BurstSubscription } from 'src/app/models/servers/subscription';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { SidebarService } from 'src/app/modules/burst/services/sidebar/sidebar.service';

/**
 * This class represents an angular component that displays on screen a list of subscribed servers.
 * @export
 * @class SidebarSelectionComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-sidebar-selection',
    templateUrl: './sidebar-selection.component.html',
    styleUrls: ['./sidebar-selection.component.scss']
})
export class SidebarSelectionComponent implements OnInit, OnDestroy {

    private subscriptions?: Subscription[] = [];

    private user?: User;

    private usersCache: { [id: string]: User[] } = {};

    public dragon = faDragon;

    public users = faUsers;

    public servers: Server[] = [];

    /**
     * Creates an instance of SidebarSelectionComponent.
     * @memberof SidebarSelectionComponent
     */
    constructor(
        private router: Router,
        private userService: UserService,
        private serversService: ServersService,
        private chatService: ChatService,
        private sidebarService: SidebarService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof SidebarSelectionComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.userService
                .user
                .subscribe(user => this.user = user),

            this.userService
                .subscriptions
                .subscribe(servers => {
                    this.servers = servers;
                    this.serversService.updateCache(servers);
                }),

            this.userService
                .usersCache
                .subscribe(cache => this.usersCache = cache),

            this.serversService
                .serverInfo
                .subscribe(server => this.serverInfoCallback(server)),

            this.chatService
                .addedServer
                .subscribe(server => this.serverInfoCallback(server)),

            this.chatService
                .updatedServer
                .subscribe(server => this.updatedServer(server)),

            this.chatService
                .subscriptionDeleted
                .subscribe(data => this.subcriptionDeletedCallback(data)),

            this.chatService
                .channelCreated
                .subscribe(data => this.channelCreatedCallback(data)),

            this.chatService
                .channelUpdated
                .subscribe(channel => this.channelUpdatedCallback(channel)),

            this.chatService
                .channelDeleted
                .subscribe(channelId => this.channelDeletedCallback(channelId)),

            this.chatService
                .updatedInvitation
                .subscribe(invite => this.updatedInvitationCallback(invite)),

            this.sidebarService
                .display
                .subscribe(options => {
                    if (options instanceof DisplayServer && options.serverId) {
                        this.serversService.set(options.serverId);
                    }
                })
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof SidebarSelectionComponent
     */
    public ngOnDestroy() {
        this.subscriptions?.forEach(s => s.unsubscribe());
    }

    /**
     * This methos is invoked when a server's information is pushed by the server info
     * observable.
     * @private
     * @param {Server} server The server instance.
     * @memberof SidebarSelectionComponent
     */
    private serverInfoCallback(server: Server) {
        if (server) {
            const index = this.servers.findIndex(s => s.id === server.id);
            if (index !== -1) {
                this.servers[index] = server;
            } else {
                this.servers.push(server);
            }
            this.serversService.updateCache(this.servers);
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
    private subcriptionDeletedCallback(data: [number, BurstSubscription]) {
        const serverId = data[0];
        const subscription = data[1];
        const server = this.servers.find(s => s.id === serverId);
        if (server && subscription) {
            const index = server
                .subscriptions
                .findIndex(s => s.userId === subscription.userId);
            if (index !== -1) {
                server.subscriptions.splice(index, 1);
                this.serversService.updateCache(this.servers);
            }
            const users = this.usersCache[server.id] || [];
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
     * @param {[number, Channel]} data The server id and the channel instance
     * @memberof SidebarSelectionComponent
     */
    private channelCreatedCallback(data: [number, Channel]) {
        const serverId = data[0];
        const channel = data[1];
        const server = this.servers.find(s => s.id === serverId);
        if (server && server.channels && server.channels.length > 0) {
            server.channels.push(channel);
            this.serversService.updateCache(this.servers);
        }
    }

    /**
     * This method is invoked when the updated information about a channel is pushed
     * by the channel updated observable.
     * @private
     * @param {Channel} channel The updated channel instance.
     * @memberof SidebarSelectionComponent
     */
    private channelUpdatedCallback(channel: Channel) {
        const server = this
            .servers
            .find(s => s.channels.some(c => c.id === channel.id));
        if (server) {
            const index = server.channels.findIndex(c => c.id === channel.id);
            if (index !== -1) {
                server.channels[index] = channel;
                this.serversService.updateCache(this.servers);
            }
        }
    }

    /**
     * This method is invoked when a channel deletion is pushed by the channel deleted
     * observable.
     * @private
     * @param {number} channelId The removed channel id.
     * @memberof SidebarSelectionComponent
     */
    private channelDeletedCallback(channelId: number) {
        const server = this
            .servers
            .find(s => s.channels.some(c => c.id === channelId));
        if (server) {
            const index = server.channels.findIndex(c => c.id === channelId);
            if (index !== -1) {
                server.channels.splice(index, 1);
                this.serversService.updateCache(this.servers);
            }
        }
    }

    /**
     * This method is invoked when an invitation is updated and pushed by the updated invitation
     * observable.
     * @private
     * @param {Invitation} invite The invitation instance.
     * @memberof SidebarSelectionComponent
     */
    private updatedInvitationCallback(invite: Invitation) {
        const inList = this.servers.some(s => s.id === invite.serverId);

        // Handle code for the user the initiated the invitation update.
        if (invite.userId === this.user.id && invite.accepted && !inList) {
            const server = invite.server;
            this.serversService.get(server.id);
            return;
        }

        // Handle code for all users of the server if the invitation was accepted.
        if (inList && invite.accepted) {
            const serverId = invite.serverId.toString();
            const server = this.servers.find(s => s.id === invite.serverId);
            const users = this.usersCache[serverId];

            users.push(invite.user);
            server.subscriptions.push({ userId: invite.userId, serverId: invite.serverId });

            this.serversService.updateCache(this.servers);
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
    public trackServerBy(index: number, server: Server) {
        return server.id;
    }

}

