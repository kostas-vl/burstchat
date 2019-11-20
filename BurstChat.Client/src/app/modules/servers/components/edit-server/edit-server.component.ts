import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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

    private serversSub?: Subscription;

    private serverInfoSub?: Subscription;

    public server?: Server;

    public newUserId = '';

    /**
     * Creates a new instance of EditServerComponent.
     * @memberof EditServerComponent
     */
    constructor(
        private activatedRoute: ActivatedRoute,
        private serversService: ServersService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditServerComponent
     */
    public ngOnInit() {
        this.serversSub = this
            .serversService
            .serverCache
            .subscribe(servers => {
                if (servers) {
                    this.findServer(servers);
                }
            });

        this.serverInfoSub = this
            .serversService
            .serverInfo
            .subscribe(server => {
                if (server && server.id === this.server.id) {
                    this.server = server;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditServerComponent
     */
    public ngOnDestroy() {
        if (this.serversSub) {
            this.serversSub
                .unsubscribe();
        }
    }

    /**
     * Fitlers the provided server list and finds the target server based on the url parameter found.
     * @private
     * @param {Server[]} servers The server list to be filtered.
     * @memberof EditServerComponent
     */
    private findServer(servers: Server[]) {
        this.activatedRoute
            .params
            .subscribe(params => {
                const id = +params.id;
                const server = servers.find(s => s.id === id);
                if (server) {
                    this.server = server;
                }
            });
    }

}
