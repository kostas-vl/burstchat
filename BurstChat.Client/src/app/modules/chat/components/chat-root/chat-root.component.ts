import { Component, OnInit, OnDestroy } from '@angular/core';
import { ChatService } from 'src/app/modules/chat/services/chat-service/chat.service';

/**
 * This class represents an angular component that displauys a series of messages between users.
 * @export
 * @class ChatRootComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-root',
    templateUrl: './chat-root.component.html',
    styleUrls: ['./chat-root.component.scss'],
    providers: [ChatService]
})
export class ChatRootComponent implements OnInit, OnDestroy {

    /**
     * Creates an instance of ChatRootComponent.
     * @memberof ChatRootComponent
     */
    constructor(private chatService: ChatService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatRootComponent
     */
    public ngOnInit() {
        this.chatService.InitializeConnection();
    }

    /**
     * Executes any necessary code for the destruction of the component.
     * @memberof ChatRootComponent
     */
    public ngOnDestroy() {
        this.chatService.DisposeConnection();
    }

}
