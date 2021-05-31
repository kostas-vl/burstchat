import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/models/chat/message';
import { faPaperPlane, faPen, faTrashAlt } from '@fortawesome/free-solid-svg-icons';

/**
 * This class represents an angular component that displays on screen a message from a user.
 * @export
 * @class ChatMessageComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-chat-message',
    templateUrl: './chat-message.component.html',
    styleUrls: ['./chat-message.component.scss']
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
    constructor() { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatMessageComponent
     */
    public ngOnInit() { }

}
