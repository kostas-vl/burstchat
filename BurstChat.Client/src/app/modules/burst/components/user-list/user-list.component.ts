import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { User } from 'src/app/models/user/user';
import { SidebarService } from 'src/app/modules/burst/services/sidebar/sidebar.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { DisplayServer } from 'src/app/models/sidebar/display-server';

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

    private displaySub?: Subscription;

    private usersCacheSub?: Subscription;

    public server?: Server;

    public users: User[];

    public loading = false;

    /**
     * Creates an instance of UserListComponent.
     * @memberof UserListComponent
     */
    constructor(
        private sidebarService: SidebarService,
        private serversService: ServersService,
        private usersService: UserService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof UserListComponent
     */
    public ngOnInit() {
        this.displaySub = this
            .sidebarService
            .display
            .subscribe(options => {
                if (options instanceof DisplayServer) {
                    const server = (options as DisplayServer).server;
                    if (server) {
                        this.server = server;
                        this.getSubscribedUsers(this.server.id);
                    }
                }
            });

        this.usersCacheSub = this
            .usersService
            .usersCache
            .subscribe(cache => {
                if (this.server) {
                    const id = this.server.id.toString();
                    this.users = cache[id] || [];
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof UserListComponent
     */
    public ngOnDestroy() {
        if (this.displaySub) {
            this.displaySub.unsubscribe();
        }

        if (this.usersCacheSub) {
            this.usersCacheSub.unsubscribe();
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
