import { Component, OnInit, OnDestroy, computed } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { DisplayServer } from 'src/app/models/sidebar/display-server';
import { ServersService } from 'src/app/services/servers/servers.service';
import { SidebarService } from 'src/app/services/sidebar/sidebar.service';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { EditServerInfoComponent } from 'src/app/components/server/edit-server-info/edit-server-info.component';
import { EditServerChannelsComponent } from 'src/app/components/server/edit-server-channels/edit-server-channels.component';
import { EditServerUsersComponent } from 'src/app/components/server/edit-server-users/edit-server-users.component';

/**
 * This class represents an angular component that presents information about the active server and enables
 * the user to edit it.
 * @class EditServerComponent
 */
@Component({
    selector: 'burst-edit-server',
    templateUrl: './edit-server.component.html',
    styleUrl: './edit-server.component.scss',
    standalone: true,
    imports: [
        CardComponent,
        CardBodyComponent,
        EditServerInfoComponent,
        EditServerChannelsComponent,
        EditServerUsersComponent
    ]
})
export class EditServerComponent implements OnInit, OnDestroy {

    private subscriptions?: Subscription[] = [];

    private routeServerId: number;

    public server = computed(() => {
        const server = this.serversService.serverInfo();
        if (server?.id === this.routeServerId) {
            return server;
        } else {
            return null;
        }
    });

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
