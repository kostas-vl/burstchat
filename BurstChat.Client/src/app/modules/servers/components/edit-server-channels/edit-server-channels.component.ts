import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { Notification } from 'src/app/models/notify/notification';
import { tryParseError } from 'src/app/models/errors/error';
import { Channel } from 'src/app/models/servers/channel';
import { Server } from 'src/app/models/servers/server';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

/**
 * This class represents an angular component that enables functionality for creating and modiyfing
 * the channels of a BurstChat server.
 * @export
 * @class EditServerChannelsComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-edit-server-channels',
    templateUrl: './edit-server-channels.component.html',
    styleUrls: ['./edit-server-channels.component.scss']
})
export class EditServerChannelsComponent implements OnInit, OnDestroy {

    private channelCreatedSub?: Subscription;

    private channelUpdatedSub?: Subscription;

    private channelDeletedSub?: Subscription;

    public newChannelName = '';

    @Input()
    public server?: Server;

    /**
     * Creates an instance of EditServerChannelsComponent.
     * @memberof EditServerChannelsComponent
     */
    constructor(
        private notifyService: NotifyService,
        private serversService: ServersService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditServerChannelsComponent
     */
    public ngOnInit() {
        this.channelCreatedSub = this
            .chatService
            .channelCreated
            .subscribe(data => {
                const serverId = data[0];
                const channel = data[1];
                if (this.server.id === serverId && this.newChannelName === channel.name) {
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
                const index = this.server.channels.findIndex(c => c.id === channelId);
                if (index > -1) {
                    this.server.channels.splice(index, 1);
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditServerChannelsComponent
     */
    public ngOnDestroy() {
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
     * Handles the add new channel button click event.
     * @memberof EditServerComponent
     */
    public onAddChannel() {
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

            this.chatService.postChannel(this.server.id, newChannel);
        }
    }

    /**
     * Handles the update channel button click event.
     * @param {Channel} channel The channel instance to be updated.
     * @memberof EditServerChannelsComponent
     */
    public onUpdateChannel(channel: Channel) {
        if (channel) {
            this.chatService.putChannel(this.server.id, channel);
        }
    }

    /**
     * Handles the delete channel button click event.
     * @param {Channel} channel The channel instance to be deleted.
     * @memberof EditServerChannelsComponent
     */
    public onDeleteChannel(channel: Channel) {
        if (channel) {
            this.chatService.deleteChannel(this.server.id, channel.id);
        }
    }

}
