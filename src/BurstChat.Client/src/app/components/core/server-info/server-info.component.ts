import { Component, computed } from '@angular/core';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faDatabase } from '@fortawesome/free-solid-svg-icons';
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
export class ServerInfoComponent {

    public database = faDatabase;

    public server = computed(() => this.serversService.serverInfo() || null);

    /**
     * Creates an instance of ServerInfoComponent.
     * @memberof ServerInfoComponent
     */
    constructor(
        private router: Router,
        private serversService: ServersService
    ) { }

    /**
     * Handles the edit server button click event.
     * @memberof ChannelListComponent
     */
    public onEditServer(): void {
        this.router.navigateByUrl(`/core/servers/edit/${this.server().id}`);
    }

}
