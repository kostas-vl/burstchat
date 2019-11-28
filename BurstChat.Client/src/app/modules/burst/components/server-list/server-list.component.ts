import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { Server } from 'src/app/models/servers/server';
import { Subscription as BurstSubscription } from 'src/app/models/servers/subscription';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { Channel } from 'src/app/models/servers/channel';
import { Invitation } from 'src/app/models/servers/invitation';

/**
 * This class represents an angular component that displays on screen a list of subscribed servers.
 * @export
 * @class ServerListComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-server-list',
    templateUrl: './server-list.component.html',
    styleUrls: ['./server-list.component.scss']
})
export class ServerListComponent implements OnInit, OnDestroy {

    private userSub?: Subscription;

    private subscribedServersSub?: Subscription;

    private usersCacheSub?: Subscription;

    private serverInfoSub?: Subscription;

    private addedServerSub?: Subscription;

    private subcriptionDeletedSub?: Subscription;

    private channelCreatedSub?: Subscription;

    private channelUpdatedSub?: Subscription;

    private channelDeletedSub?: Subscription;

    private updateInvitationSub?: Subscription;

    private user?: User;

    private usersCache: { [id: string]: User[] } = {};

    public servers: Server[] = [];

    /**
     * Creates an instance of ServerListComponent.
     * @memberof ServerListComponent
     */
    constructor(
        private router: Router,
        private userService: UserService,
        private serversService: ServersService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ServerListComponent
     */
    public ngOnInit() {
        this.userSub = this
            .userService
            .user
            .subscribe(user => this.user = user);

        this.subscribedServersSub = this
            .userService
            .subscriptions
            .subscribe(servers => {
                this.servers = servers;
                this.serversService.updateCache(servers);
            });

        this.usersCacheSub = this
            .userService
            .usersCache
            .subscribe(cache => this.usersCache = cache);

        this.serverInfoSub = this
            .serversService
            .serverInfo
            .subscribe(server => this.serverInfoCallback(server));

        this.addedServerSub = this
            .chatService
            .addedServer
            .subscribe(server => {
                this.servers.push(server);
                this.serversService.updateCache(this.servers);
            });

        this.subcriptionDeletedSub = this
            .chatService
            .subscriptionDeleted
            .subscribe(data => this.subcriptionDeletedCallback(data));

        this.channelCreatedSub = this
            .chatService
            .channelCreated
            .subscribe(data => this.channelCreatedCallback(data));

        this.channelUpdatedSub = this
            .chatService
            .channelUpdated
            .subscribe(channel => this.channelUpdatedCallback(channel));

        this.channelDeletedSub = this
            .chatService
            .channelDeleted
            .subscribe(channelId => this.channelDeletedCallback(channelId));

        this.updateInvitationSub = this
            .chatService
            .updatedInvitation
            .subscribe(invite => this.updatedInvitationCallback(invite));
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ServerListComponent
     */
    public ngOnDestroy() {
        if (this.userSub) {
            this.userSub.unsubscribe();
        }

        if (this.subscribedServersSub) {
            this.subscribedServersSub.unsubscribe();
        }

        if (this.usersCacheSub) {
            this.usersCacheSub.unsubscribe();
        }

        if (this.serverInfoSub) {
            this.serverInfoSub.unsubscribe();
        }

        if (this.addedServerSub) {
            this.addedServerSub.unsubscribe();
        }

        if (this.subcriptionDeletedSub) {
            this.subcriptionDeletedSub.unsubscribe();
        }

        if (this.channelCreatedSub) {
            this.channelCreatedSub.unsubscribe();
        }

        if (this.channelUpdatedSub) {
            this.channelUpdatedSub.unsubscribe();
        }

        if (this.channelDeletedSub) {
            this.channelDeletedSub.unsubscribe();
        }

        if (this.updateInvitationSub) {
            this.updateInvitationSub.unsubscribe();
        }
    }

    /**
     * This methos is invoked when a server's information is pushed by the server info
     * observable.
     * @private
     * @param {Server} server The server instance.
     * @memberof ServerListComponent
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
     * This method is invoked when a new value is sent from the subscription deleted
     * observale.
     * @private
     * @param {[number, BurstSubscription]} data The server id and the subscription instance.
     * @memberof ServerListComponent
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
     * @memberof ServerListComponent
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
     * @memberof ServerListComponent
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
     * @memberof ServerListComponent
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
     * @memberof ServerListComponent
     */
    private updatedInvitationCallback(invite: Invitation) {
        const notInList = !this.servers.some(s => s.id === invite.serverId);
        if (invite.userId === this.user.id && invite.accepted && notInList) {
            const server = invite.server;
            this.serversService.get(server.id);
        }
    }

    /**
     * Handles the new server button click event.
     * @memberof ServerListComponent
     */
    public onNew() {
        this.router.navigateByUrl('/core/servers/add');
    }

}

