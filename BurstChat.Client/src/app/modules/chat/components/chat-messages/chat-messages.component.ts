import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { IMessage } from 'src/app/models/chat/message';
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
            .onMessageReceivedObservable
            .subscribe(_ => this.onMessageReceived(_));
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
     * Handles the occurance of a new message sent from the server.
     * @private
     * @param {IMessage} message The message received from the server.
     * @memberof ChatMessagesComponent
     */
    private onMessageReceived(message: IMessage) {
        this.messages.push(message);
    }

}
