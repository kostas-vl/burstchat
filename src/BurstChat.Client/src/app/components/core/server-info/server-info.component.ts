import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faDatabase } from '@fortawesome/free-solid-svg-icons';
import { Server } from 'src/app/models/servers/server';
import { ServersService } from 'src/app/services/servers/servers.service';

/**
 * This class represents an angular component that displays to the users information about
 * the selected server.
 * @export
 * @class ServerInfoComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-server-info',
    templateUrl: './server-info.component.html',
    styleUrl: './server-info.component.scss',
    standalone: true,
    imports: [FontAwesomeModule]
})
export class ServerInfoComponent implements OnInit, OnDestroy {

    private serverInfoSub?: Subscription;

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
        this.serverInfoSub = this
            .serversService
            .serverInfo
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
        this.serverInfoSub?.unsubscribe();
    }

    /**
     * Handles the edit server button click event.
     * @memberof ChannelListComponent
     */
    public onEditServer(): void {
        this.router.navigateByUrl(`/core/servers/edit/${this.server.id}`);
    }

}
