import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { DirectMessagingConnectionOptions } from 'src/app/models/chat/direct-messaging-connection-options';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { DirectMessagingService } from 'src/app/modules/burst/services/direct-messaging/direct-messaging.service';

/**
 * This class represents an angular component that represents the root of the chat for
 * direct messaging.
 * @export
 * @class ChatDirectComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-direct',
    templateUrl: './chat-direct.component.html',
    styleUrls: ['./chat-direct.component.scss']
})
export class ChatDirectComponent implements OnInit, OnDestroy {

    private routeParametersSub?: Subscription;

    private onReconnectedSub?: Subscription;

    private options?: DirectMessagingConnectionOptions;

    public noChatFound = false;

    /**
     * Creates an instance of ChatDirectComponent.
     * @memberof ChatDirectComponent
     */
    constructor(
        private activatedRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private chatService: ChatService,
        private directMessagingService: DirectMessagingService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChatDirectComponent
     */
    public ngOnInit() {
        this.routeParametersSub = this
            .activatedRoute
            .queryParamMap
            .subscribe(params => {
                const users = params.getAll('user');
                if (users.length == 2) {
                    this.initiateSignalConnection([
                        +users[0], +users[1]
                    ]);
                } else {
                    this.noChatFound = true;
                    const title = 'No active chat found';
                    const content = 'Consider joining a channel or start a new private chat!';
                    this.notifyService.notify(title, content);
                }
                // const name = params.get('name');
                // const id = +params.get('id');
                // if (name !== null && id !== null) {
                //     this.options = new DirectMessagingConnectionOptions();
                //     this.options.signalGroup = `dm:${id}`;
                //     this.options.name = name;
                //     this.options.id = id;
                //     this.chatService.addSelfToChat(this.options);
                // } else {
                //     this.noChatFound = true;
                //     const title = 'No active chat found';
                //     const content = 'Consider joining a channel or start a new private chat!';
                //     this.notifyService.notify(title, content);
                // }
            });

        this.onReconnectedSub = this
            .chatService
            .onReconnected
            .subscribe(() => {
                if (this.options) {
                    this.chatService.addSelfToChat(this.options);
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChatDirectComponent
     */
    public ngOnDestroy() {
        this.routeParametersSub?.unsubscribe();
        this.onReconnectedSub?.unsubscribe();
    }

    /**
     * Executes the neccessary code in order for the direct messaging information
     * to be fetched from the API and then establish a Signal group connection.
     * @param {[number, number]} users A tuple of the user ids.
     * @memberof ChatDirectComponent
     */
    private initiateSignalConnection(users: [number, number]) {
        this.directMessagingService
            .get(users[0], users[1])
            .subscribe(directMessaging => {
                const options = new DirectMessagingConnectionOptions();
                options.signalGroup = `dm:${directMessaging.id}`;
                options.name = `${directMessaging.firstParticipantUser.name}, ${directMessaging.secondParticipantUser.name}`;
                options.id = directMessaging.id;
                options.directMessaging = directMessaging;
                this.options = options;
                this.chatService.addSelfToChat(this.options);
            });
    }

}
