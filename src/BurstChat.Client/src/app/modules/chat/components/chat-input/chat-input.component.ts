import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { Message } from 'src/app/models/chat/message';
import { User } from 'src/app/models/user/user';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

/**
 * This class represents an angular component that displays an input to which a user that send
 * a new message.
 * @export
 * @class ChatInputComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-chat-input',
    templateUrl: './chat-input.component.html',
    styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent implements OnInit, OnDestroy {

    private userSub?: Subscription;

    private allMessagesReceivedSub?: Subscription;

    private user?: User;

    private internalOptions?: ChatConnectionOptions;

    public inputContent?: string;

    public loading = true;

    public get options() {
        return this.internalOptions;
    }

    @Input()
    public set options(value: ChatConnectionOptions) {
        this.loading = true;
        this.internalOptions = value;
    }

    /**
     * Creates an instance of ChatInputComponent.
     * @memberof ChatInputComponent
     */
    constructor(
        private userService: UserService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatInputComponent
     */
    public ngOnInit() {
        this.userSub= this
            .userService
            .user
            .subscribe(user => {
                if (user) {
                    this.user = user;
                }
            });

        this.allMessagesReceivedSub= this
            .chatService
            .allMessagesReceived
            .subscribe(_ => {
                setTimeout(() => this.loading = false, 300);
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChatInputComponent
     */
    public ngOnDestroy() {
        this.userSub?.unsubscribe();
        this.allMessagesReceivedSub?.unsubscribe();
    }

    /**
     * When the enter key is pressed in the message box and the input content has a value a new message is sent
     * to the connected signal server.
     * @memberof ChatInputComponent
     */
    public onEnterKeyPressed() {
        if (this.user && this.inputContent) {
            const message: Message = {
                id: 0,
                userId: this.user.id,
                user: null,
                content: this.inputContent,
                datePosted: new Date(),
                edited: false,
                links: []
            };

            this.chatService.postMessage(this.options, message);
            this.inputContent = undefined;
        }
    }

}
