import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { DisplayServer } from 'src/app/models/sidebar/display-server';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { SidebarService } from 'src/app/modules/burst/services/sidebar/sidebar.service';

/**
 * This class represents an angular component that presents information about the active server and enables
 * the user to edit it.
 * @class EditServerComponent
 */
@Component({
    selector: 'burst-edit-server',
    templateUrl: './edit-server.component.html',
    styleUrls: ['./edit-server.component.scss']
})
export class EditServerComponent implements OnInit, OnDestroy {

    private subscriptions?: Subscription[] = [];

    private routeServerId: number;

    public server?: Server;

    /**
     * Creates a new instance of EditServerComponent.
     * @memberof EditServerComponent
     */
    constructor(
        private activatedRoute: ActivatedRoute,
        private serversService: ServersService,
        private sidebarService: SidebarService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditServerComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.activatedRoute
                .params
                .subscribe(params => {
                    this.routeServerId = +params.id;
                    const displayServer = new DisplayServer(this.routeServerId);
                    this.sidebarService.toggleDisplay(displayServer);
                }),
            this.serversService
                .serverInfo
                .subscribe(server => {
                    if (server && server.id === this.routeServerId) {
                        this.server = server;
                    }
                })
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditServerComponent
     */
    public ngOnDestroy() {
        this.subscriptions?.forEach(s => s.unsubscribe());
    }

}
