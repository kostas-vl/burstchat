import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { Message } from 'src/app/models/chat/message';
import { MessageCluster } from 'src/app/models/chat/message-cluster';
import { Payload } from 'src/app/models/signal/payload';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

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

    private selfAddedToChatSubscription?: Subscription;

    private allMessagesReceivedSubscription?: Subscription;

    private messageReceivedSubscription?: Subscription;

    private internalOptions?: ChatConnectionOptions;

    public topIndex = 0;

    public bottomIndex = 0;

    public messages: MessageCluster[] = [];

    @Input()
    public set options(value: ChatConnectionOptions) {
        this.internalOptions = value;
        this.unsubscribeAll();

        this.selfAddedToChatSubscription = this
            .chatService
            .selfAddedToChat
            .subscribe(() => this.onSelfAddedToChat());

        this.allMessagesReceivedSubscription = this
            .chatService
            .allMessagesReceived
            .subscribe(payload => this.onMessagesReceived(payload));

        this.messageReceivedSubscription = this
            .chatService
            .messageReceived
            .subscribe(payload => this.onMessageReceived(payload));
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
     * @private
     * @memberof ChatMessagesComponent
     */
    private unsubscribeAll(): void {
        if (this.selfAddedToChatSubscription) {
            this.selfAddedToChatSubscription
                .unsubscribe();
        }

        if (this.allMessagesReceivedSubscription) {
            this.allMessagesReceivedSubscription
                .unsubscribe();
        }

        if (this.messageReceivedSubscription) {
            this.messageReceivedSubscription
                .unsubscribe();
        }
    }

    /**
     * This method will evaluate if a message was posted in date and time close to the provided cluster date.
     * @private
     * @param {(Date | string)} clusterDate The date the cluster of messages was posted.
     * @param {(Date | string)} messageDate The date the message was posted.
     * @returns A boolean specifying if the date difference is greater that expected.
     * @memberof ChatMessagesComponent
     */
    private isSameDateTime(clusterDate: Date | string, messageDate: Date | string) {
        const clusterProperDate: Date = clusterDate instanceof Date
            ? clusterDate
            : new Date(clusterDate);
        const messageProperDate: Date = messageDate instanceof Date
            ? messageDate
            : new Date(messageDate);
        const isSameDay =
            clusterProperDate.getFullYear() === messageProperDate.getFullYear()
            && clusterProperDate.getMonth() === messageProperDate.getMonth()
            && clusterProperDate.getDate() === messageProperDate.getDate();

        return isSameDay;
    }

    /**
     * This method will add a new message to the appropriate cluster or create a new one based on the provided parameters.
     * This method mutates the provided cluster list.
     * @private
     * @param {MessageCluster[]} clusterList The cluster list with all the current messsages.
     * @param {Message} message The message posted.
     * @returns The modified cluster list.
     * @memberof ChatMessagesComponent
     */
    private addMessageToCluster(clusterList: MessageCluster[], message: Message) {
        const lastEntry = clusterList[clusterList.length - 1];

        const isPartOfTheCluster = lastEntry
            && lastEntry.user.id === message.userId
            && this.isSameDateTime(lastEntry.datePosted, message.datePosted);

        if (isPartOfTheCluster) {
            lastEntry
                .messages
                .push(message);
        } else {
            clusterList.push({
                user: message.user,
                datePosted: message.datePosted,
                messages: [message]
            });
        }

        return clusterList;
    }

    /**
     * Handles the successful connection to a chat.
     * @private
     * @memberof ChatMessagesComponent
     */
    private onSelfAddedToChat(): void {
        this.chatService.getAllMessages(this.internalOptions);
    }

    /**
     * Handles the entire payload of messages posted to a chat.
     * @private
     * @param {Payload<Message[]>} payload A payload with the messages received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessagesReceived(payload: Payload<Message[]>): void {
        const messages = this.internalOptions.signalGroup === payload.signalGroup
            ? payload.content
            : [];

        if (messages.length > 0) {
            const initialValue: MessageCluster[] = [];
            this.messages = messages.reduce((current, next, _) => this.addMessageToCluster(current, next), initialValue);
        } else {
            this.messages = [];
        }
    }

    /**
     * Handles the payload of a new message posted to the chat.
     * @private
     * @param {Payload<Message>} payload The payload with the new message received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessageReceived(payload: Payload<Message>): void {
        const message = this.internalOptions.signalGroup === payload.signalGroup
            ? payload.content
            : null;

        if (message) {
            this.messages = this.addMessageToCluster(this.messages, message);
        }
    }

}
