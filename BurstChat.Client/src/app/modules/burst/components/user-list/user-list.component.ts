import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { User } from 'src/app/models/user/user';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { UserService } from 'src/app/modules/burst/services/user/user.service';

/**
 * This class represents an angular component that displays the list of users that are subscribed to a server.
 * @export
 * @class UserListComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit, OnDestroy {

    private activeServerSubscription?: Subscription;

    public server?: Server;

    public users: User[];

    public loading = false;

    /**
     * Creates an instance of UserListComponent.
     * @memberof UserListComponent
     */
    constructor(
        private serversService: ServersService,
        private usersService: UserService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof UserListComponent
     */
    public ngOnInit() {
        this.activeServerSubscription = this
            .serversService
            .activeServer
            .subscribe(server => {
                if (server) {
                    this.server = server;
                    this.getSubscribedUsers(this.server.id);
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof UserListComponent
     */
    public ngOnDestroy() {
        if (this.activeServerSubscription) {
            this.activeServerSubscription
                .unsubscribe();
        }
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
