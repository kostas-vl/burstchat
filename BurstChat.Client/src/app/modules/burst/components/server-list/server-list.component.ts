import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { Server } from 'src/app/models/servers/server';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';

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

    private activeServerSub?: Subscription;

    private serverInfoSub?: Subscription;

    private addedServerSub?: Subscription;

    private subcriptionDeletedSub?: Subscription;

    private channelCreatedSub?: Subscription;

    private channelUpdatedSub?: Subscription;

    private channelDeletedSub?: Subscription;

    private updateInvitationSub?: Subscription;

    private user?: User;

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
            .subscribe(user => {
                if (user) {
                    this.user = user;
                }
            });

        this.subscribedServersSub = this
            .userService
            .subscriptions
            .subscribe(servers => {
                this.servers = servers;
                this.serversService.updateCache(servers);
            });

        this.activeServerSub = this
            .serversService
            .activeServer
            .subscribe(server => {
                let serverInList = this.servers.find(s => server && s.id === server.id);
                if (serverInList) {
                    serverInList = server;
                }
            });

        this.serverInfoSub = this
            .serversService
            .serverInfo
            .subscribe(server => {
                if (server) {
                    const index = this.servers.findIndex(s => s.id === server.id);
                    if (index > -1) {
                        this.servers[index] = server;
                        this.serversService.updateCache(this.servers);
                    }
                }
            });

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
            .subscribe(data => {
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
                }
            });

        this.channelCreatedSub = this
            .chatService
            .channelCreated
            .subscribe(data => {
                const serverId = data[0];
                const channel = data[1];
                const server = this.servers.find(s => s.id === serverId);
                if (server && server.channels && server.channels.length > 0) {
                    server.channels.push(channel);
                    this.serversService.updateCache(this.servers);
                }
            });

        this.channelUpdatedSub = this
            .chatService
            .channelUpdated
            .subscribe(channel => {
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
            });

        this.channelDeletedSub = this
            .chatService
            .channelDeleted
            .subscribe(channelId => {
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
            });

        this.updateInvitationSub = this
            .chatService
            .updatedInvitation
            .subscribe(invite => {
                const notInList = !this.servers.some(s => s.id === invite.serverId);
                if (invite.userId === this.user.id && invite.accepted && notInList) {
                    const server = invite.server;
                    this.serversService.get(server.id);
                }
            });
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

        if (this.activeServerSub) {
            this.activeServerSub.unsubscribe();
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
     * Handles the new server button click event.
     * @memberof ServerListComponent
     */
    public onNew() {
        this.router.navigateByUrl('/core/servers/add');
    }

}

