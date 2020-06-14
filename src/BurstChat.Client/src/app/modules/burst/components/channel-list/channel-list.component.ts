import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { SidebarService } from '../../services/sidebar/sidebar.service';
import { DisplayServer } from 'src/app/models/sidebar/display-server';

/**
 * This class represents an angular component that displays the list of channel that exist in a BurstChat
 * server.
 * @class ChannelListComponent
 */
@Component({
    selector: 'app-channel-list',
    templateUrl: './channel-list.component.html',
    styleUrls: ['./channel-list.component.scss']
})
export class ChannelListComponent implements OnInit, OnDestroy {

    private queryParamsSub?: Subscription;

    private displaySub?: Subscription;

    private serverInfoSub?: Subscription;

    public activeChannelId?: number;

    public server?: Server;

    /**
     * Creates a new instance of ChannelListComponent.
     * @memberof ChannelListComponent
     */
    constructor(
        private router: Router,
        private sidebarService: SidebarService,
        private activatedRoute: ActivatedRoute,
        private serversService: ServersService,
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChannelListComponent
     */
    public ngOnInit() {
        this.queryParamsSub = this
            .activatedRoute
            .queryParamMap
            .subscribe(params => {
                const isChatUrl = this
                    .router
                    .url
                    .includes('chat/channel');
                const channelId: number | null = +params.get('id');
                this.activeChannelId = isChatUrl && channelId !== null
                    ? channelId
                    : undefined;
            });

        this.displaySub = this
            .sidebarService
            .display
            .subscribe(options => {
                if (options instanceof DisplayServer) {
                    const server = (options as DisplayServer).server;
                    if (server) {
                        this.server = server;
                        if (server.channels.length === 0) {
                            this.serversService.get(server.id);
                        }
                    }
                }
            });

        this.serverInfoSub = this
            .serversService
            .serverInfo
            .subscribe(server => {
                const canAssign = server
                    && this.server
                    && this.server.id === server.id;
                if (canAssign) {
                    this.server = server;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChannelListComponent
     */
    public ngOnDestroy() {
        if (this.queryParamsSub) {
            this.queryParamsSub.unsubscribe();
        }

        if (this.displaySub) {
            this.displaySub.unsubscribe();
        }

        if (this.serverInfoSub) {
            this.serverInfoSub.unsubscribe();
        }
    }

}