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

    private userSubscription?: Subscription;

    private subscribedServersSubscription?: Subscription;

    private activeServerSubscription?: Subscription;

    private addedServerSubscription?: Subscription;

    private updateInvitationSubscription?: Subscription;

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
        this.userSubscription = this
            .userService
            .user
            .subscribe(user => {
                if (user) {
                    this.user = user;
                }
            });

        this.subscribedServersSubscription = this
            .userService
            .subscriptions
            .subscribe(servers => this.servers = servers);

        this.activeServerSubscription = this
            .serversService
            .activeServer
            .subscribe(server => {
                let serverInList = this.servers.find(s => server && s.id === server.id);
                if (serverInList) {
                    serverInList = server;
                }
            });

        this.addedServerSubscription = this
            .chatService
            .addedServer
            .subscribe(server => this.servers.push(server));

        this.updateInvitationSubscription = this
            .chatService
            .updatedInvitation
            .subscribe(invite => {
                const notInList = !this.servers.some(s => s.id === invite.serverId);
                if (invite.userId === this.user.id && invite.accepted && notInList) {
                    const server = invite.server;
                    this.getServer(server);
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ServerListComponent
     */
    public ngOnDestroy() {
        if (this.userSubscription) {
            this.userSubscription
                .unsubscribe();
        }

        if (this.subscribedServersSubscription) {
            this.subscribedServersSubscription
                .unsubscribe();
        }

        if (this.activeServerSubscription) {
            this.activeServerSubscription
                .unsubscribe();
        }

        if (this.addedServerSubscription) {
            this.addedServerSubscription
                .unsubscribe();
        }

        if (this.updateInvitationSubscription) {
            this.updateInvitationSubscription
                .unsubscribe();
        }
    }

    /**
     * Fetches information about a server based on the provided instance.
     * @private
     * @param {Server} server The server instance.
     * @memberof ServerListComponent
     */
    private getServer(server: Server) {
        this.serversService
            .get(server.id)
            .subscribe(entry => {
                if (entry) {
                    this.servers.push(entry);
                }
            });
    }

    /**
     * Handles the new server button click event.
     * @memberof ServerListComponent
     */
    public onNew(): void {
        this.router.navigateByUrl('/core/servers/add');
    }

}

