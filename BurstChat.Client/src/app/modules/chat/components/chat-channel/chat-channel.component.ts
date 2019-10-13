import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, Subscribable } from 'rxjs';
import { Notification } from 'src/app/models/notify/notification';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

/**
 * This class represents an angular component that is the root component that contains a server channel chat.
 * @export
 * @class ChatChannelComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-channel',
    templateUrl: './chat-channel.component.html',
    styleUrls: ['./chat-channel.component.scss']
})
export class ChatChannelComponent implements OnInit, OnDestroy {

    private routeParametersSubscription?: Subscription;

    private onReconnectedSubscription?: Subscription;

    private options?: ChannelConnectionOptions;

    public noChatFound = false;

    /**
     * Creates an instance of ChatChannelComponent.
     * @memberof ChatChannelComponent
     */
    constructor(
        private activatedRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChatChannelComponent
     */
    public ngOnInit() { 
        this.routeParametersSubscription = this
            .activatedRoute
            .queryParamMap
            .subscribe(params => {
                const name = params.get('name');
                const id = +params.get('id');
                if (name !== null && id !== null) {
                    this.options = new ChannelConnectionOptions();
                    this.options.signalGroup = `channel:${id}`;
                    this.options.name = name;
                    this.options.id = id;
                    this.chatService.addSelfToChat(this.options);
                } else {
                    this.noChatFound = true;
                    const notification: Notification = {
                        title: 'No active chat found',
                        content: 'Consider joining a channel or start a new private chat!'
                    };
                    this.notifyService.notify(notification);
                }
            });

        this.onReconnectedSubscription = this
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
     * @memberof ChatChannelComponent
     */
    public ngOnDestroy() {
        if (this.routeParametersSubscription) {
            this.routeParametersSubscription
                .unsubscribe();
        }

        if (this.onReconnectedSubscription) {
            this.onReconnectedSubscription
                .unsubscribe();
        }
    }

}
