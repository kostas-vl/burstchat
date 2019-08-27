import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { IMessage } from 'src/app/models/chat/message';
import { Payload } from 'src/app/models/signal/payload';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { ChatService } from 'src/app/modules/chat/services/chat-service/chat.service';

/**
 * This class represents an angular component that displays on screen the messages of the chat.
 * @export
 * @class ChatMessagesComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-messages',
    templateUrl: './chat-messages.component.html',
    styleUrls: ['./chat-messages.component.scss']
})
export class ChatMessagesComponent implements OnInit, OnDestroy {

    private onMessageReceivedSubscription?: Subscription;

    public topIndex = 0;

    public bottomIndex = 0;

    public messages: IMessage[] = [];

    @Input()
    public options?: PrivateGroupConnectionOptions | ChannelConnectionOptions;

    /**
     * Creates an instance of ChatMessagesComponent.
     * @memberof ChatMessagesComponent
     */
    constructor(private chatService: ChatService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatMessagesComponent
     */
    public ngOnInit() {
        this.onMessageReceivedSubscription = this
            .chatService
            .onAllPrivateGroupMessagesReceived
            .subscribe(payload => this.onMessageReceived(payload));
    }

    /**
     * Executes any necessary code for the destruction of the component.
     * @memberof ChatMessagesComponent
     */
    public ngOnDestroy() {
        if (this.onMessageReceivedSubscription) {
            this.onMessageReceivedSubscription
                .unsubscribe();
        }
    }

    /**
     * Handles the entire payload of messages posted to a chat.
     * @private
     * @param {Payload<IMessage[]>} payload A payload with the messages received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessageReceived(payload: Payload<IMessage[]>) {
        const signalGroupId = +payload.signalGroup;

        if (this.options instanceof PrivateGroupConnectionOptions) {
            if (this.options.privateGroupId === signalGroupId)
                this.messages = payload.content;
            return;
        }

        if (this.options instanceof ChannelConnectionOptions) {
            if (this.options.channelId === signalGroupId)
                this.messages = payload.content;
            return;
        }
    }

}
