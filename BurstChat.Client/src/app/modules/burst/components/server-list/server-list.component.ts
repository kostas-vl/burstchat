import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { UserService } from 'src/app/modules/burst/services/user/user.service';

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

    private subscribedServersSubscription?: Subscription;

    public servers: Server[] = [];

    /**
     * Creates an instance of ServerListComponent.
     * @memberof ServerListComponent
     */
    constructor(private userService: UserService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ServerListComponent
     */
    public ngOnInit() {
        this.subscribedServersSubscription = this
            .userService
            .subscriptionsObservable
            .subscribe(servers => this.servers = servers);
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ServerListComponent
     */
    public ngOnDestroy() {
        if (this.subscribedServersSubscription) {
            this.subscribedServersSubscription
                .unsubscribe();
        }
    }

}

