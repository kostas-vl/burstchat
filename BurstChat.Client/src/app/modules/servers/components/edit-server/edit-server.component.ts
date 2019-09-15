import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';

/**
 * This class represents an angular component that presents information about the active server and enables
 * the user to edit it.
 * @class EditServerComponent
 */
@Component({
  selector: 'app-edit-server',
  templateUrl: './edit-server.component.html',
  styleUrls: ['./edit-server.component.scss']
})
export class EditServerComponent implements OnInit, OnDestroy {

    private activeServerSubscription?: Subscription;

    public server?: Server;

    public newUserId = '';

    /**
     * Creates a new instance of EditServerComponent.
     * @memberof EditServerComponent
     */
    constructor(private serversService: ServersService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditServerComponent
     */
    public ngOnInit() {
        this.activeServerSubscription = this
            .serversService
            .activeServer
            .subscribe(server => {
                if (server) {
                    this.server = server;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditServerComponent
     */
    public ngOnDestroy() {
        if (this.activeServerSubscription) {
            this.activeServerSubscription
                .unsubscribe();
        }
    }

}
