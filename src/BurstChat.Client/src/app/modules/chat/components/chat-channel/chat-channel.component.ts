import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { ChatLayoutService } from 'src/app/modules/chat/services/chat-layout/chat-layout.service';

/**
 * This class represents an angular component that is the root component that contains a server channel chat.
 * @export
 * @class ChatChannelComponent
 * @implements {OnInit, OnDestroy}
 */
@Component({
    selector: 'app-chat-channel',
    templateUrl: './chat-channel.component.html',
    styleUrls: ['./chat-channel.component.scss'],
    providers: [ChatLayoutService]
})
export class ChatChannelComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    private options?: ChannelConnectionOptions;

    public noChatFound = false;

    public layoutState: 'chat' | 'call' = 'chat';

    /**
     * Creates an instance of ChatChannelComponent.
     * @memberof ChatChannelComponent
     */
    constructor(
        private activatedRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private chatService: ChatService,
        private chatLayoutService: ChatLayoutService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChatChannelComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.activatedRoute
                .queryParamMap
                .subscribe(params => {
                    const name = params.get('name');
                    const id = +params.get('id');
                    if (name !== null && id !== null) {
                        this.options = new ChannelConnectionOptions();
                        this.options.signalGroup = `channel:${id}`;
                        this.options.name = name;
                        this.options.id = id;
                    } else {
                        this.noChatFound = true;
                        const title = 'No active chat found';
                        const content = 'Consider joining a channel or start a new private chat!';
                        this.notifyService.notify(title, content);
                    }
                }),

            this.chatService
                .onReconnected
                .subscribe(() => {
                    if (this.options) {
                        this.chatService.addSelfToChat(this.options);
                    }
                }),

            this.chatLayoutService
                .toggle$
                .subscribe(s => this.layoutState = s)
        ];

    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChatChannelComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }

}
