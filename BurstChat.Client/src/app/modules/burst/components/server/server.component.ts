import { Component, OnInit, Input } from '@angular/core';
import { Server } from 'src/app/models/servers/server';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';

/**
 * This class represents an angular component that displays on screen a subscribed server.
 * @export
 * @class ServerComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-server',
    templateUrl: './server.component.html',
    styleUrls: ['./server.component.scss']
})
export class ServerComponent implements OnInit {

    @Input()
    public server?: Server;

    public get serverInitials(): string {
        if (this.server) {
            return this
                .server
                .name
                .slice(0, 1)
                .toUpperCase();
        }

        return '';
    }

    /**
     * Creates an instance of ServerComponent.
     * @memberof ServerComponent
     */
    constructor(private serversService: ServersService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ServerComponent
     */
    public ngOnInit() { }

    /**
     * Handles the server button click event.
     * @memberof ServerComponent
     */
    public onSelect() {
        if (this.server) {
            this.serversService
                .setActiveServer(this.server);
        }
    }

}
