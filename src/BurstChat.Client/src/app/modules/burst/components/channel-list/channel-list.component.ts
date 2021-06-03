import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Channel } from 'src/app/models/servers/channel';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';

/**
 * This class represents an angular component that displays the list of channel that exist in a BurstChat
 * server.
 * @class ChannelListComponent
 */
@Component({
    selector: 'burst-channel-list',
    templateUrl: './channel-list.component.html',
    styleUrls: ['./channel-list.component.scss']
})
export class ChannelListComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    public activeChannelId?: number;

    public channels: Channel[] = [];

    public loading = true;

    /**
     * Creates a new instance of ChannelListComponent.
     * @memberof ChannelListComponent
     */
    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private serversService: ServersService,
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChannelListComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.activatedRoute
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
                }),

            this.serversService
                .serverInfo
                .subscribe(server => {
                    if (server) {
                        this.channels = server?.channels || [];
                    }

                    this.loading = false;
                })
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChannelListComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }

}
