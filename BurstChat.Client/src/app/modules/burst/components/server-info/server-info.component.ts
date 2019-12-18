import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { faDatabase } from '@fortawesome/free-solid-svg-icons';
import { Server } from 'src/app/models/servers/server';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';

/**
 * This class represents an angular component that displays to the users information about
 * the selected server.
 * @export
 * @class ServerInfoComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-server-info',
    templateUrl: './server-info.component.html',
    styleUrls: ['./server-info.component.scss']
})
export class ServerInfoComponent implements OnInit, OnDestroy {

    private activeServerSub?: Subscription;

    public database = faDatabase;

    public server?: Server;

    /**
     * Creates an instance of ServerInfoComponent.
     * @memberof ServerInfoComponent
     */
    constructor(
        private router: Router,
        private serversService: ServersService
    ) { }

    /**
     * Executest any neccessary start up code for the component.
     * @memberof ServerInfoComponent
     */
    public ngOnInit() {
        this.activeServerSub = this
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
     * @memberof ServerInfoComponent
     */
    public ngOnDestroy() {
        if (this.activeServerSub) {
            this.activeServerSub.unsubscribe();
        }
    }

    /**
     * Handles the edit server button click event.
     * @memberof ChannelListComponent
     */
    public onEditServer(): void {
        this.router.navigateByUrl(`/core/servers/edit/${this.server.id}`);
    }

}
