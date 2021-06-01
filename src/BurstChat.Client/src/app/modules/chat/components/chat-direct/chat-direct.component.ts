import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Message } from 'src/app/models/chat/message';
import { DirectMessagingConnectionOptions } from 'src/app/models/chat/direct-messaging-connection-options';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { DirectMessagingService } from 'src/app/modules/burst/services/direct-messaging/direct-messaging.service';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';
import { ChatLayoutService } from 'src/app/modules/chat/services/chat-layout/chat-layout.service';
import { ChatDialogService } from 'src/app/modules/chat/services/chat-dialog/chat-dialog.service';

/**
 * This class represents an angular component that represents the root of the chat for
 * direct messaging.
 * @export
 * @class ChatDirectComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-chat-direct',
    templateUrl: './chat-direct.component.html',
    styleUrls: ['./chat-direct.component.scss'],
    providers: [ChatLayoutService]
})
export class ChatDirectComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    private internalOptions?: DirectMessagingConnectionOptions;

    public noChatFound = false;

    public layoutState: 'chat' | 'call' = 'chat';

    public get options() {
        return this.internalOptions;
    }

    public editMessageData: { visible: boolean, message?: Message } = { visible: false, message: null };

    public deleteMessageData: { visible: boolean, message?: Message} = { visible: false, message: null};

    /**
     * Creates an instance of ChatDirectComponent.
     * @memberof ChatDirectComponent
     */
    constructor(
        private activatedRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private chatService: ChatService,
        private directMessagingService: DirectMessagingService,
        private chatLayoutService: ChatLayoutService,
        private chatDialogService: ChatDialogService,
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChatDirectComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.activatedRoute
                .queryParamMap
                .subscribe(params => {
                    const users = params.getAll('user');
                    if (users.length === 2) {
                        this.initiateSignalConnection([
                            +users[0], +users[1]
                        ]);
                        const state = params.get('display');
                        if (state === 'chat' || state === 'call'){
                            this.chatLayoutService.toggle(state);
                        }
                    } else {
                        this.noChatFound = true;
                        const title = 'No active chat found';
                        const content = 'Consider joining a channel or start a new private chat!';
                        this.notifyService.notify(title, content);
                    }
                }),
            this.chatService
                .onReconnected$
                .subscribe(() => {
                    if (this.options) {
                        this.chatService.addSelfToChat(this.options);
                    }
                }),
            this.chatLayoutService
                .toggle$
                .subscribe(s => this.layoutState = s),
            this.chatDialogService
                .editMessage$
                .subscribe(message => this.editMessageData = {
                    visible: true,
                    message: message,
                }),
            this.chatDialogService
                .deleteMessage$
                .subscribe(message => this.deleteMessageData = {
                    visible: true,
                    message: message,
                })
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChatDirectComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
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
                this.internalOptions = options;
            });
    }

}
