import { Component, EventEmitter, OnInit, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Message } from 'src/app/models/chat/message';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { ChatService } from 'src/app/services/chat/chat.service';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';

/**
 * This class represents an angular component that presents a dialog about deleting a chat message.
 * @class MessageDeleteDialogComponent
 */
@Component({
    selector: 'burst-message-delete-dialog',
    templateUrl: './message-delete-dialog.component.html',
    styleUrl: './message-delete-dialog.component.scss',
    standalone: true,
    imports: [
        FormsModule,
        DialogComponent,
        CardComponent,
        CardBodyComponent,
        CardFooterComponent
    ]
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
