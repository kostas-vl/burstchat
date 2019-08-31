import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { tryParseError } from 'src/app/models/errors/error';
import { Notification } from 'src/app/models/notify/notification';
import { Server } from 'src/app/models/servers/server';
import { Channel } from 'src/app/models/servers/channel';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { ChannelsService } from 'src/app/modules/burst/services/channels/channels.service';

/**
 * This class represents an angular component that presents information about the active server and enables
 * the user to edit it.
 * @class EditServerComponent
 */
@Component({
  selector: 'app-edit-server',
  templateUrl: './edit-server.component.html',
  styleUrls: ['./edit-server.component.scss']
})
export class EditServerComponent implements OnInit {

    private activeServerSubscription?: Subscription;

    public server?: Server;

    public newChannelName = '';

    /**
     * Creates a new instance of EditServerComponent.
     * @memberof EditServerComponent
     */
    constructor(
        private notifyService: NotifyService,
        private serversService: ServersService,
        private channelsService: ChannelsService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditServerComponent
     */
    public ngOnInit(): void {
        this.activeServerSubscription = this
            .serversService
            .activeServerObservable
            .subscribe(server => {
                if (server) {
                    this.server = server;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditServerComponent
     */
    public ngOnDestroy(): void {
        if (this.activeServerSubscription) {
            this.activeServerSubscription
                .unsubscribe();
        }
    }

    /**
     * Performs a request for server information about the current edited server inorder to
     * properly update it.
     * @memberof EditServerComponent
     */
    private onFetchUpdatedServer(): void {
        if (this.server) {
            this.serversService
            .get(this.server.id)
            .subscribe(
                server => {
                    const notification: Notification = {
                        title: 'Success',
                        content: `The channel ${this.newChannelName} was created successfully.`
                    };
                    this.notifyService.notify(notification);
                    this.serversService.setActiveServer(server);
                },
                httpError => {
                    const error = tryParseError(httpError.error);
                    this.notifyService.notifyError(error);
                }
            );
        }
    }

    /**
     * Handles the add new channel button click event.
     * @memberof EditServerComponent
     */
    public onAddChannel(): void {
        if (!this.newChannelName) {
            const notification: Notification = {
                title: 'Could not create channel',
                content: 'Please provide a name for the new channel!'
            };
            this.notifyService.notify(notification);
            return;
        }

        if (this.server) {
            const newChannel: Channel = {
                id: 0,
                name: this.newChannelName,
                isPublic: true,
                dateCreated: new Date(),
                details: null
            };

            this.channelsService
                .post(this.server.id, newChannel)
                .subscribe(
                    () => this.onFetchUpdatedServer(),
                    httpError => {
                        const error = tryParseError(httpError.error);
                        this.notifyService.notifyError(error);
                    }
                );
        }
    }

}