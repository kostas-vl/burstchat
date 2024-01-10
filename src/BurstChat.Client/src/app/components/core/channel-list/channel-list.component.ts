import { Component, OnInit, OnDestroy, computed } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Channel } from 'src/app/models/servers/channel';
import { ServersService } from 'src/app/services/servers/servers.service';
import { ExpanderComponent } from 'src/app/components/shared/expander/expander.component';
import { ChannelComponent } from 'src/app/components/core/channel/channel.component';

/**
 * This class represents an angular component that displays the list of channel that exist in a BurstChat
 * server.
 * @class ChannelListComponent
 */
@Component({
    selector: 'burst-channel-list',
    templateUrl: './channel-list.component.html',
    styleUrl: './channel-list.component.scss',
    standalone: true,
    imports: [ExpanderComponent, ChannelComponent]
})
export class ChannelListComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    public activeChannelId?: number;

    public channels = computed(() => {
        const channels = this.serversService.serverInfo()?.channels || [];
        this.loading = false;
        return channels;
    });

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
