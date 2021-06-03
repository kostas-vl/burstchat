import { Component, EventEmitter, OnInit, Input, Output } from '@angular/core';
import { Message } from 'src/app/models/chat/message';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

@Component({
    selector: 'burst-message-edit-dialog',
    templateUrl: './message-edit-dialog.component.html',
    styleUrls: ['./message-edit-dialog.component.scss']
})
export class MessageEditDialogComponent implements OnInit {

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
     * Creates a new instance of MessageEditDialogComponent.
     * @memberof MessageEditDialogComponent
     */
    constructor(private chatService: ChatService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof MessageEditDialogComponent
     */
    public ngOnInit() { }

    /**
     * Sets the default values to various fields.
     * @private
     * @memberof MessageEditDialogComponent
     */
    private reset() {
        this.visible = false;
        this.visibleChange.emit(this.visible);
        this.content = '';
    }

    /**
     * Handles the save button click event.
     * @memberof MessageEditDialogComponent
     */
    public onSave() {
        this.internalMessage = {
            ...this.internalMessage,
            user: { ...this.internalMessage.user, avatar: '' },
            content: this.content
        };
        this.chatService.editMessage(this.options, this.internalMessage);
        this.reset();
    }

    /**
     * Handles the cancel button click event.
     * @memberof MessageEditDialogComponent
     */
    public onCancel() {
        this.reset();
    }

}
