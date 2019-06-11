import { Component, OnInit } from '@angular/core';
import { ChatService } from 'src/app/modules/chat/services/chat-service/chat.service';
import { IMessage } from 'src/app/models/chat/message';

/**
 * This class represents an angular component that displays an input to which a user that send
 * a new message.
 * @export
 * @class ChatInputComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-input',
    templateUrl: './chat-input.component.html',
    styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent implements OnInit {

    public inputContent?: string;

    /**
     * Creates an instance of ChatInputComponent.
     * @memberof ChatInputComponent
     */
    constructor(private chatService: ChatService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatInputComponent
     */
    public ngOnInit() { }

    /**
     * When the enter key is pressed in the message box and the input content has a value a new message is sent
     * to the connected signal server.
     * @memberof ChatInputComponent
     */
    public onEnterKeyPressed() {
        if (this.inputContent) {
            const message: IMessage = {
                user: 'hoodi',
                content: this.inputContent,
                datePosted: new Date(),
                edited: false
            };

            this.chatService.sendMessage(message);

            this.inputContent = undefined;
        }
    }

}
