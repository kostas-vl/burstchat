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
 * This class represents an angular component that presents a dialog about changing a chat message.
 * @class MessageEditDialogComponent
 */
@Component({
    selector: 'burst-message-edit-dialog',
    templateUrl: './message-edit-dialog.component.html',
    styleUrl: './message-edit-dialog.component.scss',
    standalone: true,
    imports: [
        FormsModule,
        DialogComponent,
        CardComponent,
        CardBodyComponent,
        CardFooterComponent
    ]
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
