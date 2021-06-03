import { Component, EventEmitter, OnInit, Input, Output } from '@angular/core';
import { Message } from 'src/app/models/chat/message';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

@Component({
    selector: 'burst-message-delete-dialog',
    templateUrl: './message-delete-dialog.component.html',
    styleUrls: ['./message-delete-dialog.component.scss']
})
export class MessageDeleteDialogComponent implements OnInit {

    private internalMessage?: Message;

    public isVisible = false;

    public content = '';

    @Input()
    public options?: ChatConnectionOptions;

    @Input()
    public set message(value: Message) {
        if (value) {
            this.internalMessage = value;
            this.content = value.content;
        }
    }

    @Input()
    public set visible(value: boolean) {
        this.isVisible = value;
    }

    @Output()
    public visibleChange = new EventEmitter<boolean>();

    /**
     * Creates a new instance of MessageDeleteDialogComponent.
     * @memberof MessageDeleteDialogComponent
     */
    constructor(private chatService: ChatService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof MessageDeleteDialogComponent
     */
    public ngOnInit() { }

    /**
     * Sets the default value to various fields.
     * @private
     * @memberof MessageDeleteDialogComponent
     */
    private reset() {
        this.visible = false;
        this.visibleChange.emit(this.visible);
        this.internalMessage = null;
        this.content = '';
    }

    /**
     * Handles the delete button click event.
     * @memberof MessageDeleteDialogComponent
     */
    public onDelete() {
        const message = {
            ...this.internalMessage,
            user: {
                ...this.internalMessage.user,
                avatar: '',
            },
        }
        this.chatService.deleteMessage(this.options, message);
        this.reset();
    }

    /**
     * Handles the cancel button click event.
     * @memberof MessageDeleteDialogComponent
     */
    public onCancel() {
        this.reset();
    }

}
