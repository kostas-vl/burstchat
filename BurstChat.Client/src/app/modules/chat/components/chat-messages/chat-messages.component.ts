import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { Message } from 'src/app/models/chat/message';
import { MessageCluster } from 'src/app/models/chat/message-cluster';
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

    public messages: MessageCluster[] = [];

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
     * @private
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

    private isSameDay(clusterDate: Date | string, messageDate: Date | string) {
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

        if (lastEntry && lastEntry.user.id === message.userId && this.isSameDay(lastEntry.datePosted, message.datePosted)) {
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
        let messages: Message[] = [];
        if (this.internalOptions instanceof PrivateGroupConnectionOptions) {
            const signalGroup = +payload
                .signalGroup
                .split('privateGroup:')[1];
            if (this.internalOptions.privateGroupId === signalGroup) {
                messages = payload.content;
            }
        }

        if (this.internalOptions instanceof ChannelConnectionOptions) {
            const signalGroup = +payload
                .signalGroup
                .split('channel:')[1];
            if (this.internalOptions.channelId === signalGroup) {
                messages = payload.content;
            }
        }

        if (messages.length > 0) {
            this.messages = messages
                .reduce((previous, current, _) => this.addMessageToCluster(previous, current), ([] as MessageCluster[]));
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
        let message: Message;

        if (this.internalOptions instanceof PrivateGroupConnectionOptions) {
            const signalGroup = +payload
                .signalGroup
                .split('privateGroup:')[1];
            if (this.internalOptions.privateGroupId === signalGroup) {
                message = payload.content;
            }
        }

        if (this.internalOptions instanceof ChannelConnectionOptions) {
            const signalGroup = +payload
                .signalGroup
                .split('channel:')[1];
            if (this.internalOptions.channelId === signalGroup) {
                message = payload.content;
            }
        }

        if (message) {
            this.messages = this.addMessageToCluster(this.messages, message);
        }
    }

}
