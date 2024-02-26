import { Component, Input, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Message } from 'src/app/models/chat/message';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { UserService } from 'src/app/services/user/user.service';
import { ChatService } from 'src/app/services/chat/chat.service';

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
    styleUrl: './chat-input.component.scss',
    standalone: true,
    imports: [FormsModule]
})
export class ChatInputComponent {

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
    ) {
        effect(() => {
            if (this.chatService.allMessagesReceived()) {
                setTimeout(() => this.loading = false, 300);
            }
        })
    }

    /**
     * When the enter key is pressed in the message box and the input content has a value a new message is sent
     * to the connected signal server.
     * @memberof ChatInputComponent
     */
    public onEnterKeyPressed() {
        const user = this.userService.user();
        if (user && this.inputContent) {
            const message: Message = {
                id: 0,
                userId: user.id,
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
