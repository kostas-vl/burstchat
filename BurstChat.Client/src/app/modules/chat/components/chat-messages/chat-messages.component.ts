import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { Message } from 'src/app/models/chat/message';
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

    private onSelfAddedToGroupSubscription?: Subscription;

    private onAllMessagesReceivedSubscription?: Subscription;

    private onMessageReceivedSubscription?: Subscription;

    private internalOptions?: PrivateGroupConnectionOptions | ChannelConnectionOptions;

    public topIndex = 0;

    public bottomIndex = 0;

    public messages: Message[] = [];

    @Input()
    public set options(value: PrivateGroupConnectionOptions | ChannelConnectionOptions) {
        this.internalOptions = value;
        this.unsubscribeAll();

        if (this.internalOptions instanceof PrivateGroupConnectionOptions) {
            this.onSelfAddedToGroupSubscription = this
                .chatService
                .onSelfAddedToPrivateGroup
                .subscribe(() => this.onSelfAddedToGroup());

            this.onAllMessagesReceivedSubscription = this
                .chatService
                .onAllPrivateGroupMessagesReceived
                .subscribe(payload => this.onMessagesReceived(payload));

            this.onMessageReceivedSubscription = this
                .chatService
                .onPrivateGroupMessageReceived
                .subscribe(payload => this.onMessageReceived(payload));
        }

        if (this.internalOptions instanceof ChannelConnectionOptions) {
            this.onSelfAddedToGroupSubscription = this
                .chatService
                .onSelfAddedToChannel
                .subscribe(() => this.onSelfAddedToGroup());

            this.onAllMessagesReceivedSubscription = this
                .chatService
                .onAllChannelMessagesReceived
                .subscribe(payload => this.onMessagesReceived(payload));

            this.onMessageReceivedSubscription = this
                .chatService
                .onChannelMessageReceived
                .subscribe(payload => this.onMessageReceived(payload));
        }
    }

    /**
     * Creates an instance of ChatMessagesComponent.
     * @memberof ChatMessagesComponent
     */
    constructor(private chatService: ChatService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatMessagesComponent
     */
    public ngOnInit(): void { }

    /**
     * Executes any necessary code for the destruction of the component.
     * @memberof ChatMessagesComponent
     */
    public ngOnDestroy(): void {
        this.unsubscribeAll();
    }

    /**
     * Unsubscribes all instanciated subscriptions of the component.
     * @memberof ChatMessagesComponent
     */
    private unsubscribeAll(): void {
        if (this.onSelfAddedToGroupSubscription) {
            this.onSelfAddedToGroupSubscription
                .unsubscribe();
        }

        if (this.onAllMessagesReceivedSubscription) {
            this.onAllMessagesReceivedSubscription
                .unsubscribe();
        }

        if (this.onMessageReceivedSubscription) {
            this.onMessageReceivedSubscription
                .unsubscribe();
        }
    }

    /**
     * Handles the successful connection to a signal group.
     * @private
     * @memberof ChatMessagesComponent
     */
    private onSelfAddedToGroup(): void {
        if (this.internalOptions instanceof PrivateGroupConnectionOptions) {
            const id = this.internalOptions.privateGroupId;
            this.chatService.getAllPrivateGroupMessages(id);
            return;
        }

        if (this.internalOptions instanceof ChannelConnectionOptions) {
            const id = this.internalOptions.channelId;
            this.chatService.getAllChannelMessages(id);
            return;
        }
    }

    /**
     * Handles the entire payload of messages posted to a chat.
     * @private
     * @param {Payload<Message[]>} payload A payload with the messages received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessagesReceived(payload: Payload<Message[]>): void {
        console.log(payload);
        if (this.internalOptions instanceof PrivateGroupConnectionOptions) {
            const signalGroup = +payload
                .signalGroup
                .split('privateGroup:')[1];
            if (this.internalOptions.privateGroupId === signalGroup)
                this.messages = payload.content;
            return;
        }

        if (this.internalOptions instanceof ChannelConnectionOptions) {
            const signalGroup = +payload
                .signalGroup
                .split('channel:')[1];
            if (this.internalOptions.channelId === signalGroup)
                this.messages = payload.content;
            return;
        }
    }

    /**
     * Handles the payload of a new message posted to the chat.
     * @private
     * @param {Payload<Message>} payload The payload with the new message received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessageReceived(payload: Payload<Message>): void {
        console.log(payload);
        if (this.internalOptions instanceof PrivateGroupConnectionOptions) {
            const signalGroup = +payload
                .signalGroup
                .split('privateGroup:')[1];
            if (this.internalOptions.privateGroupId === signalGroup) {
                const message = payload.content;
                this.messages.push(message);
            }
            return;
        }

        if (this.internalOptions instanceof ChannelConnectionOptions) {
            const signalGroup = +payload
                .signalGroup
                .split('channel:')[1];
            if (this.internalOptions.channelId === signalGroup) {
                const message = payload.content;
                this.messages.push(message);
            }
            return;
        }
    }

}
