import { Component, OnInit, OnDestroy, Input, ViewChild } from '@angular/core';
import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
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

    private scrollBottomOffset = 0;

    public loadingMessages = true;

    public topIndex = 0;

    public bottomIndex = 0;

    public messagesClusters: MessageCluster[] = [];

    public chatIsEmpty = false;

    @Input()
    public set options(value: ChatConnectionOptions) {
        this.messagesClusters = [];
        this.chatIsEmpty = false;
        this.loadingMessages = true;
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

    @ViewChild(CdkVirtualScrollViewport, { static: false })
    public viewport?: CdkVirtualScrollViewport;

    /**
     * Creates an instance of ChatMessagesComponent.
     * @memberof ChatMessagesComponent
     */
    constructor(private chatService: ChatService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatMessagesComponent
     */
    public ngOnInit() { }

    /**
     * Executes any necessary code for the destruction of the component.
     * @memberof ChatMessagesComponent
     */
    public ngOnDestroy() {
        this.unsubscribeAll();
    }

    /**
     * Unsubscribes all instanciated subscriptions of the component.
     * @private
     * @memberof ChatMessagesComponent
     */
    private unsubscribeAll() {
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
     * This method will scroll the virtual scroll viewport to the bottom based
     * on the number of messages available.
     * @private
     * @memberof ChatMessagesComponent
     */
    private scrollToBottom() {
        const canScrollToBottom = this.viewport && this.scrollBottomOffset <= 100;

        if (canScrollToBottom) {
            this.viewport.scrollTo({ bottom: 0 });
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
     * The clust is always fetches from the top of the list.
     * This method mutates the provided cluster list and returns it.
     * @private
     * @param {MessageCluster[]} clusterList The cluster list with all the current messsages.
     * @param {Message} message The message posted.
     * @returns The modified cluster list.
     * @memberof ChatMessagesComponent
     */
    private addMessageToClustersTop(clusterList: MessageCluster[], message: Message) {
        const firstEntry = clusterList[0];
        const isPartOfTheCluster =
            firstEntry
            && firstEntry.user.id === message.userId
            && this.isSameDateTime(firstEntry.datePosted, message.datePosted);

        if (isPartOfTheCluster) {
            firstEntry.messages = [message, ...firstEntry.messages];
        } else {
            clusterList = [
                {
                    user: message.user,
                    datePosted: message.datePosted,
                    messages: [message]
                },
                ...clusterList
            ];
        }

        return clusterList;
    }

    /**
     * This method will add a new message to the appropriate cluster or create a new one based on the provided parameters.
     * The clust is always fetches from the bottom of the list.
     * This method mutates the provided cluster list and returns it.
     * @private
     * @param {MessageCluster[]} clusterList The cluster list with all the current messsages.
     * @param {Message} message The message posted.
     * @returns The modified cluster list.
     * @memberof ChatMessagesComponent
     */
    private addMessageToClustersBottom(clusterList: MessageCluster[], message: Message) {
        const lastEntry = clusterList[clusterList.length - 1];
        const isPartOfTheCluster =
            lastEntry
            && lastEntry.user.id === message.userId
            && this.isSameDateTime(lastEntry.datePosted, message.datePosted);

        if (isPartOfTheCluster) {
            lastEntry.messages.push(message);
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
     * This method will add a new message to the appropriate cluster or create a new one based on the provided parameters.
     * This method mutates the provided cluster list and returns it.
     * @private
     * @param {MessageCluster[]} clusterList The cluster list with all the current messsages.
     * @param {Message} message The message posted.
     * @returns The modified cluster list.
     * @memberof ChatMessagesComponent
     */
    private addMessageToClusters(clusterList: MessageCluster[], message: Message) {
        const firstEntry = clusterList[0];
        const firstMessage = firstEntry && firstEntry.messages
            ? firstEntry.messages[0]
            : null;

        if (firstMessage && firstMessage.id > message.id) {
            this.addMessageToClustersTop(clusterList, message);
        } else {
            this.addMessageToClustersBottom(clusterList, message);
        }

        return clusterList;
    }

    /**
     * This method will add a list of new messages to the appropriate cluster.
     * This method mutates the provided cluster list and returns it.
     * @private
     * @param {MessageCluster[]} clusterList The cluster list with all the current messages.
     * @param {Message[]} messages The messages posted.
     * @returns The modified cluster list.
     * @memberof ChatMessagesComponent
     */
    private addMessagesToClusters(clusterList: MessageCluster[], messages: Message[]) {
        const firstCluster = clusterList[0];
        const firstClusterMessage = firstCluster && firstCluster.messages
            ? firstCluster.messages[0]
            : null;

        const firstMessage = messages[0];

        // This check is done to ensure that the messages will be pushed to the clusters
        // from oldest to the newest message when inserted to the top of the list
        // or from the newest to the older when inserted to the bottom.
        const reverseIteration =
            firstClusterMessage
            && firstMessage
            && firstClusterMessage.id > firstMessage.id;

        const start = reverseIteration ? messages.length - 1 : 0;
        const endCallback = reverseIteration
            ? (x) => x > 0
            : (x) => x < messages.length;
        const interval = reverseIteration ? -1 : 1;

        for (let i = start; endCallback(i); i += interval) {
            clusterList = this.addMessageToClusters(clusterList, messages[i]);
        }

        return clusterList;
    }

    /**
     * Handles the successful connection to a chat.
     * @private
     * @memberof ChatMessagesComponent
     */
    private onSelfAddedToChat() {
        this.chatService.getAllMessages(this.internalOptions);
    }

    /**
     * Handles the entire payload of messages posted to a chat.
     * @private
     * @param {Payload<Message[]>} payload A payload with the messages received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessagesReceived(payload: Payload<Message[]>) {
        const messages = this.internalOptions.signalGroup === payload.signalGroup
            ? payload.content
            : [];

        if (messages.length > 0) {
            this.messagesClusters = this.addMessagesToClusters([...this.messagesClusters], messages);
        } else if (messages.length === 0 && this.messagesClusters.length === 0) {
            this.chatIsEmpty = true;
        }

        setTimeout(() => {
            this.scrollToBottom();
            this.loadingMessages = false;
        }, 50);
    }

    /**
     * Handles the payload of a new message posted to the chat.
     * @private
     * @param {Payload<Message>} payload The payload with the new message received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessageReceived(payload: Payload<Message>) {
        const message = this.internalOptions.signalGroup === payload.signalGroup
            ? payload.content
            : null;

        if (message) {
            this.messagesClusters = this.addMessageToClusters([...this.messagesClusters], message);
            this.chatIsEmpty = false;
        }

        setTimeout(() => this.scrollToBottom(), 50);
    }

    /**
     * Handles the new index that was set on the virtual scroll viewport.
     * @private
     * @memberof ChatMessagesComponent
     */
    public onScrolledIndexChanged(event: any) {
        this.scrollBottomOffset = this.viewport.measureScrollOffset('bottom');

        const scrollTopOffset = this.viewport.measureScrollOffset('top');
        const canRequestMessages = this.messagesClusters
            && !this.loadingMessages
            && scrollTopOffset <= 100;

        if (canRequestMessages) {
            const oldestMessage = this.messagesClusters[0].messages[0] || null;
            const messageId = oldestMessage.id || null;

            this.chatService.getAllMessages(this.internalOptions, messageId);
            this.loadingMessages = true;
        }
    }

}
