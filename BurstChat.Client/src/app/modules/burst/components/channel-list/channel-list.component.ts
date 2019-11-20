import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

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

    private activeServerSub?: Subscription;

    private serverInfoSub?: Subscription;

    private channelCreatedSub?: Subscription;

    private channelUpdatedSub?: Subscription;

    private channelDeletedSub?: Subscription;

    public activeChannelId?: number;

    public server?: Server;

    /**
     * Creates a new instance of ChannelListComponent.
     * @memberof ChannelListComponent
     */
    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private serversService: ServersService,
        private chatService: ChatService
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
                if (isChatUrl && channelId !== null) {
                    this.activeChannelId = channelId;
                } else {
                    this.activeChannelId = undefined;
                }
            });

        this.activeServerSub = this
            .serversService
            .activeServer
            .subscribe(server => {
                if (server) {
                    this.server = server;
                    if (server.channels.length === 0) {
                        this.serversService.get(server.id);
                    }
                }
            });

        this.serverInfoSub = this
            .serversService
            .serverInfo
            .subscribe(server => {
                if (server && this.server.id === server.id) {
                    this.server = server;
                }
            });

        this.channelCreatedSub = this
            .chatService
            .channelCreated
            .subscribe(data => {
                const serverId = data[0];
                const channel = data[1];
                if (this.server.id === serverId) {
                    this.server.channels.push(channel);
                }
            });

        this.channelUpdatedSub = this
            .chatService
            .channelUpdated
            .subscribe(channel => {
                if (channel) {
                    const index = this.server.channels.findIndex(c => c.id === channel.id);
                    if (index > -1) {
                        this.server.channels[index] = channel;
                    }
                }
            });

        this.channelDeletedSub = this
            .chatService
            .channelDeleted
            .subscribe(channelId => {
                if (channelId) {
                    const index = this.server.channels.findIndex(c => c.id === channelId);
                    if (index > -1) {
                        this.server.channels.splice(index, 1);
                    }
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

        if (this.activeServerSub) {
            this.activeServerSub.unsubscribe();
        }

        if (this.serverInfoSub) {
            this.serverInfoSub.unsubscribe();
        }

        if (this.channelCreatedSub) {
            this.channelCreatedSub.unsubscribe();
        }

        if (this.channelUpdatedSub) {
            this.channelUpdatedSub.unsubscribe();
        }

        if (this.channelDeletedSub) {
            this.channelDeletedSub.unsubscribe();
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
