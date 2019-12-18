import { Component, OnInit, OnDestroy, Input, ViewChild } from '@angular/core';
import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { Subscription } from 'rxjs';
import { Message } from 'src/app/models/chat/message';
import { Payload } from 'src/app/models/signal/payload';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { NotifyService } from 'src/app/services/notify/notify.service';

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

    public messages: Message[] = [];

    public chatIsEmpty = false;

    @Input()
    public set options(value: ChatConnectionOptions) {
        this.messages = [];
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
    constructor(
        private chatService: ChatService,
        private notifyService: NotifyService
    ) { }

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
     * @param {(Date | string)} messageDate The date a message that was posted.
     * @param {(Date | string)} newMessageDate The date of the new message that was posted.
     * @returns A boolean specifying if the date difference is greater that expected.
     * @memberof ChatMessagesComponent
     */
    private isSameDateTime(messageDate: Date | string, newMessageDate: Date | string) {
        const messageProperDate: Date = messageDate instanceof Date
            ? messageDate
            : new Date(messageDate);

        const newMessageProperDate: Date = newMessageDate instanceof Date
            ? newMessageDate
            : new Date(newMessageDate);

        const isSameDay =
            messageProperDate.getFullYear() === newMessageProperDate.getFullYear()
            && messageProperDate.getMonth() === newMessageProperDate.getMonth()
            && messageProperDate.getDate() === newMessageProperDate.getDate();

        return isSameDay;
    }

    /**
     * This method will add a new message to the messages list either at the start or end.
     * This method mutates the provided messages list and returns it.
     * @private
     * @param {Message[]} messages The current messages.
     * @param {Message} message The message posted.
     * @returns The modified messages list.
     * @memberof ChatMessagesComponent
     */
    private addMessageToClusters(messages: Message[], message: Message) {
        const first = messages[0];
        const isSameDay = first
            ? this.isSameDateTime(first.datePosted, message.datePosted)
            : false;

        if (first && first.id > message.id) {
            messages = [message, ...messages];
        } else {
            messages.push(message);
        }

        return messages;
    }

    /**
     * This method will add a list of new batch of messages to the position in the provided messages list.
     * This method mutates the messages list and returns it.
     * @private
     * @param {Message[]} messages The current messages list.
     * @param {Message[]} newBatch The new messages posted.
     * @returns The modified messages list.
     * @memberof ChatMessagesComponent
     */
    private addMessagesToClusters(messages: Message[], newBatch: Message[]) {
        const first = messages[0];
        const firstFromBatch = newBatch[0];

        // This check is done to ensure that the messages will be pushed to the clusters
        // from oldest to the newest message when inserted to the top of the list
        // or from the newest to the older when inserted to the bottom.
        const reverseIteration = first && firstFromBatch && first.id > firstFromBatch.id;

        const start = reverseIteration ? messages.length - 1 : 0;
        const endCallback = reverseIteration
            ? (x) => x > 0
            : (x) => x < newBatch.length;
        const interval = reverseIteration ? -1 : 1;

        for (let i = start; endCallback(i); i += interval) {
            messages = this.addMessageToClusters(messages, newBatch[i]);
        }

        return messages;
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
        const newBatch = this.internalOptions.signalGroup === payload.signalGroup
            ? payload.content
            : [];

        if (newBatch.length > 0) {
            this.messages = this.addMessagesToClusters([...this.messages], newBatch);
        } else if (newBatch.length === 0 && this.messages.length === 0) {
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
            this.messages = this.addMessageToClusters([...this.messages], message);
            this.chatIsEmpty = false;
            this.notifyService.notify('New message', message.content);
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
        const canRequestMessages = this.messages
            && !this.loadingMessages
            && scrollTopOffset <= 100;

        if (canRequestMessages) {
            const oldestMessage = this.messages[0] || null;
            const messageId = oldestMessage.id || null;

            this.chatService.getAllMessages(this.internalOptions, messageId);
            this.loadingMessages = true;
        }
    }

}
