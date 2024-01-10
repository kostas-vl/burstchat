import { Component, OnDestroy, Input, ViewChild, effect } from '@angular/core';
import { Subscription } from 'rxjs';
import { VirtualScrollerComponent, IPageInfo, VirtualScrollerModule } from '@iharbeck/ngx-virtual-scroller';
import { Message } from 'src/app/models/chat/message';
import { User } from 'src/app/models/user/user';
import { Payload } from 'src/app/models/signal/payload';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { ChatService } from 'src/app/services/chat/chat.service';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { UiLayerService } from 'src/app/services/ui-layer/ui-layer.service';
import { ChatMessageComponent } from 'src/app/components/chat/chat-message/chat-message.component';

/**
 * This class represents an angular component that displays on screen the messages of the chat.
 * @export
 * @class ChatMessagesComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-chat-messages',
    templateUrl: './chat-messages.component.html',
    styleUrl: './chat-messages.component.scss',
    standalone: true,
    imports: [
        VirtualScrollerModule,
        ChatMessageComponent
    ]
})
export class ChatMessagesComponent implements OnDestroy {

    private subscriptions?: Subscription[] = [];

    private internalOptions?: ChatConnectionOptions;

    public loadingMessages = true;

    public searchTerm?: string;

    public messages: Message[] = [];

    public chatIsEmpty = false;

    public canScrollToBottom = true;

    public inCall = true;

    public get options() {
        return this.internalOptions;
    }

    @ViewChild(VirtualScrollerComponent)
    public scroller: VirtualScrollerComponent;

    @Input()
    public set options(value: ChatConnectionOptions) {
        this.messages = [];
        this.chatIsEmpty = false;
        this.loadingMessages = true;
        this.internalOptions = value;
        this.unsubscribeAll();

        this.subscriptions = [
            this.uiLayerService
                .search$
                .subscribe(term => this.onSearch(term)),
        ];

        this.chatService.addSelfToChat(this.options);
    }

    /**
     * Creates an instance of ChatMessagesComponent.
     * @memberof ChatMessagesComponent
     */
    constructor(
        private chatService: ChatService,
        private notifyService: NotifyService,
        private uiLayerService: UiLayerService
    ) {
        effect(() => this.onUserUpdated(this.chatService.userUpdated()));
        effect(() => this.onSelfAddedToChat(this.chatService.selfAddedToChat()));
        effect(() => this.onMessagesReceived(this.chatService.allMessagesReceived()));
        effect(() => this.onMessageReceived(this.chatService.messageReceived()));
        effect(() => this.onMessageEdited(this.chatService.messageEdited()));
        effect(() => this.onMessageDeleted(this.chatService.messageDeleted()));
    }

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
        this.subscriptions.forEach(s => s.unsubscribe());
    }

    /**
     * This method will scroll the virtual scroll viewport to the bottom based
     * on the number of messages available.
     * @private
     * @memberof ChatMessagesComponent
     */
    private scrollToBottom() {
        if (!this.canScrollToBottom) return;
        const index = this.messages.length - 1;
        this.scroller.scrollToIndex(index);
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
     * Checks how the source message relates to the relative message based on user ids
     * and dates posted and set the sources display mode.
     * @param {Message} source The source message.
     * @param {Message} relative The relative previous message.
     * @returns {Message} A copy of the source with the display mode set.
     * @memberof ChatMessagesComponent
     */
    private getMessageWithDisplayMode(source: Message, relative: Message): Message {
        if (relative) {
            const isSameDateTime = this.isSameDateTime(relative.datePosted, source.datePosted);
            if (relative.userId === source.userId && isSameDateTime) {
                return { ...source, displayMode: 'simple' };
            }
        }

        return { ...source, displayMode: 'full' };
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

        if (first && first.id > message.id) {
            const msg = this.getMessageWithDisplayMode(message, messages[0]);
            messages = [msg, ...messages];
        } else {
            const msg = this.getMessageWithDisplayMode(message, messages[messages?.length - 1]);
            messages.push(msg);
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
     * @param {boolean} added The flag indicating if the current user was added to chat connection.
     * @memberof ChatMessagesComponent
     */
    private onSelfAddedToChat(added: boolean) {
        if (this.options && added) {
            this.chatService.getAllMessages(this.options);
        }
    }

    /**
     * Handles the entire payload of messages posted to a chat.
     * @private
     * @param {Payload<Message[]>} payload A payload with the messages received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessagesReceived(payload: Payload<Message[]> | null) {
        if (!this.options || !payload) return;

        const newBatch = this.options.signalGroup === payload.signalGroup
            ? payload.content
            : [];

        if (newBatch.length > 0) {
            this.messages = this.addMessagesToClusters([...this.messages], newBatch);
            this.chatIsEmpty = false;
            this.scrollToBottom();
        } else if (newBatch.length === 0 && this.messages.length === 0) {
            this.chatIsEmpty = true;
        }

        this.loadingMessages = false;
    }

    /**
     * Handles the payload of a new message posted to the chat.
     * @private
     * @param {Payload<Message>} payload The payload with the new message received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessageReceived(payload: Payload<Message> | null) {
        if (!this.options || !payload) return;

        const message = this.options.signalGroup === payload.signalGroup
            ? payload.content
            : null;

        if (!message) return;

        this.messages = this.addMessageToClusters([...this.messages], message);
        this.chatIsEmpty = false;
        this.scrollToBottom();
        this.notifyService.notify(`${message.user.name} sent:`, message.content);
    }

    /**
     * Handles the payload of a message that was edited.
     * @param {Payload<Message> | null} message The message that was edited.
     * @memberof ChatMessageComponent
     */
    private onMessageEdited(payload: Payload<Message> | null) {
        if (!this.options || !payload) return;

        const message = this.options.signalGroup === payload.signalGroup
            ? payload.content
            : null;

        if (!message) return;

        const instance = this.messages.find(m => m.id === message.id);
        instance.content = message.content;
        instance.edited = message.edited;
    }

    /**
     * Handles the payload of a message that was deleted from the chat.
     * @param {Payload<Message> | null} message The payload with the deleted message.
     * @memberof ChatMessagesComponent
     */
    private onMessageDeleted(payload: Payload<Message> | null) {
        if (!this.options || !payload) return;

        const message= this.options.signalGroup === payload.signalGroup
            ? payload.content
            : null;

        if (!message) return;

        const instance = this.messages.find(m => m.id === message.id);
        if (instance) {
            const indexOf = this.messages.indexOf(instance);
            this.messages.splice(indexOf, 1);
        }
    }

    private onSearch(searchTerm?: string) {
        if (this.loadingMessages) return;

        this.messages = [];
        this.searchTerm = searchTerm;
        this.chatService.getAllMessages(this.options, this.searchTerm);
        this.loadingMessages = true;
    }

    /**
     * Checks if the updated user that was sent is in any of the messages.
     * @param {User | null} user The updated user instance.
     * @memberof ChatMessagesComponent
     */
    public onUserUpdated(user: User | null) {
        if (!this.options || !user) return;

        const messages = this.messages.filter(m => m.user.id === user.id);
        for (const message of messages) {
            message.user = { ...user };
        }
    }

    /**
     * Handles the scroll change event of the virtul scroller.
     * @private
     * @memberof ChatMessagesComponent
     */
    public onVirtualScrollerChange(page: IPageInfo) {
        if (page.startIndex === 0) {
            const canRequestMessages = this.messages && !this.loadingMessages;
            if (canRequestMessages) {
                const oldestMessage = this.messages[0] || null;
                const messageId = oldestMessage.id || null;

                this.chatService.getAllMessages(this.options, this.searchTerm, messageId);
                this.loadingMessages = true;
            }
        }

        this.canScrollToBottom = page.endIndex === (this.messages.length - 1);
    }

}
