import { Component, Input, effect } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Channel } from 'src/app/models/servers/channel';
import { Server } from 'src/app/models/servers/server';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/services/chat/chat.service';

/**
 * This class represents an angular component that enables functionality for creating and modiyfing
 * the channels of a BurstChat server.
 * @export
 * @class EditServerChannelsComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-edit-server-channels',
    templateUrl: './edit-server-channels.component.html',
    styleUrl: './edit-server-channels.component.scss',
    standalone: true,
    imports: [
        FormsModule,
        DatePipe
    ]
})
export class EditServerChannelsComponent {

    public newChannelName = '';

    @Input()
    public server?: Server;

    /**
     * Creates an instance of EditServerChannelsComponent.
     * @memberof EditServerChannelsComponent
     */
    constructor(
        private notifyService: NotifyService,
        private chatService: ChatService
    ) {
        effect(() => {
            const channel = this.chatService.channelCreated();
            if (channel && this.newChannelName === channel[1].name) {
                this.newChannelName = '';
            }
        })
    }

    /**
     * Handles the add new channel button click event.
     * @memberof EditServerComponent
     */
    public onAddChannel() {
        if (!this.newChannelName) {
            const title = 'Could not create channel';
            const content = 'Please provide a name for the new channel!';
            this.notifyService.notify(title, content);
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
