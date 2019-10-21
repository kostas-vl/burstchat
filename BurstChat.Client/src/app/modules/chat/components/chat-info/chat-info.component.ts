import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { DirectMessagingConnectionOptions } from 'src/app/models/chat/direct-messaging-connection-options';

/**
 * This class represents an angular component that displays on screen the top bar of the application
 * that provides various types of functionality and actions.
 * @export
 * @class ChatInfoComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-info',
    templateUrl: './chat-info.component.html',
    styleUrls: ['./chat-info.component.scss']
})
export class ChatInfoComponent implements OnInit {

    @Input()
    public options?: ChatConnectionOptions;

    public get icon() {
        if (this.options instanceof ChannelConnectionOptions) {
            return 'lni-bubble';
        } else if (this.options instanceof PrivateGroupConnectionOptions) {
            return 'lni-network';
        } else if (this.options instanceof DirectMessagingConnectionOptions) {
            return 'lni-users';
        } else {
            return '';
        }
    }

    /**
     * Creates a new instance of ChatInfoComponent.
     * @memberof ChatInfoComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChatInfoComponent
     */
    public ngOnInit(): void { }



}
