import { Component, OnInit, Input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPaperPlane, faPen, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { Message } from 'src/app/models/chat/message';
import { UiLayerService } from 'src/app/modules/chat/services/ui-layer/ui-layer.service';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';

/**
 * This class represents an angular component that displays on screen a message from a user.
 * @export
 * @class ChatMessageComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-chat-message',
    templateUrl: './chat-message.component.html',
    styleUrl: './chat-message.component.scss',
    standalone: true,
    imports: [
        DatePipe,
        FontAwesomeModule,
        AvatarComponent
    ]
})
export class ChatMessageComponent implements OnInit {

    public plane = faPaperPlane;

    public edit = faPen;

    public delete = faTrashAlt;

    public get fullDisplay() {
        return this.message?.displayMode === 'full';
    }

    @Input()
    public message?: Message;

    /**
     * Creates an instance of ChatMessageComponent.
     * @memberof ChatMessageComponent
     */
    constructor(private uiLayerService: UiLayerService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatMessageComponent
     */
    public ngOnInit() { }

    /**
     * Handles the message edit button click event.
     * @memberof ChatMessageComponent
     */
    public onEdit() {
        this.uiLayerService.editMessage(this.message);
    }

    /**
     * Handles the message delete button click event.
     * @memberof ChatMessageComponent
     */
    public onDelete() {
        this.uiLayerService.deleteMessage(this.message);
    }

}
