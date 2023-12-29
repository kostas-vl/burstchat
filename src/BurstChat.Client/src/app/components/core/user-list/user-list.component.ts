import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { User } from 'src/app/models/user/user';
import { ServersService } from 'src/app/services/servers/servers.service';
import { UserService } from 'src/app/services/user/user.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { ExpanderComponent } from 'src/app/components/shared/expander/expander.component';
import { UserComponent } from 'src/app/components/core/user/user.component';

/**
 * This class represents an angular component that displays the list of users that are subscribed to a server.
 * @export
 * @class UserListComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-user-list',
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.scss',
    standalone: true,
    imports: [
        ExpanderComponent,
        UserComponent
    ]
})
export class UserListComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    public server?: Server;

    public users: User[] = [];

    public loading = false;

    /**
     * Creates an instance of UserListComponent.
     * @memberof UserListComponent
     */
    constructor(
        private serversService: ServersService,
        private usersService: UserService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof UserListComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.serversService
                .serverInfo
                .subscribe(server => {
                    if (server) {
                        this.server = server;
                        const users = this.usersService.getFromCache(this.server.id);
                        if (!users) {
                            this.getSubscribedUsers(this.server.id);
                        } else {
                            this.users = users;
                        }
                    }
                }),
            this.usersService
                .usersCache
                .subscribe(cache => {
                    if (this.server) {
                        const id = this.server.id.toString();
                        this.users = cache[id] || [];
                    }
                }),
            this.chatService
                .userUpdated$
                .subscribe(user => {
                    const entry = this.users.find(u => u.id === user.id);
                    if (entry) {
                        const index = this.users.indexOf(entry);
                        this.users[index] = { ...user };
                    }
                })
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof UserListComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }

    /**
     * Fetches the subscribed users of the server.
     * @private
     * @memberof UserListComponent
     */
    private getSubscribedUsers(serverId: number) {
        this.loading = true;
        this.users = [];

        this.serversService
            .getSubscribedUsers(serverId)
            .subscribe(users => {
                this.users = users;
                this.usersService.pushToCache(this.server.id, this.users);
                this.loading = false;
            }, error => {
                this.loading = false;
            });
    }

}
