import { Component, OnInit, Input } from '@angular/core';
import { IMessage } from 'src/app/models/chat/message';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { StorageService } from 'src/app/services/storage/storage.service';
import { ChatService } from 'src/app/modules/chat/services/chat-service/chat.service';

/**
 * This class represents an angular component that displays an input to which a user that send
 * a new message.
 * @export
 * @class ChatInputComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-input',
    templateUrl: './chat-input.component.html',
    styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent implements OnInit {

    public inputContent?: string;

    @Input()
    public options?: PrivateGroupConnectionOptions | ChannelConnectionOptions;

    /**
     * Creates an instance of ChatInputComponent.
     * @memberof ChatInputComponent
     */
    constructor(
        private storageService: StorageService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatInputComponent
     */
    public ngOnInit() { }

    /**
     * When the enter key is pressed in the message box and the input content has a value a new message is sent
     * to the connected signal server.
     * @memberof ChatInputComponent
     */
    public onEnterKeyPressed() {
        if (this.inputContent) {
            const message: IMessage = {
                userId: 0,
                content: this.inputContent,
                datePosted: new Date(),
                edited: false
            };

            if (this.options instanceof PrivateGroupConnectionOptions) {
                const groupId = this.options.privateGroupId;
                this.chatService.postPrivateGroupMessage(groupId, message);
                this.inputContent = undefined;
                return;
            }

            if (this.options instanceof ChannelConnectionOptions) {
                const channelId  = this.options.channelId;
                this.chatService.postChannelMessage(channelId, message);
                this.inputContent = undefined;
                return;
            }
        }
    }

}
