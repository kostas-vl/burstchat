import { Component, OnInit, Input } from '@angular/core';
import { MessageCluster } from 'src/app/models/chat/message-cluster';

/**
 * This class represents an angular component that displays on screen a message from a user.
 * @export
 * @class ChatMessageComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-message',
    templateUrl: './chat-message.component.html',
    styleUrls: ['./chat-message.component.scss']
})
export class ChatMessageComponent implements OnInit {

    @Input()
    public messageCluster?: MessageCluster;

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
