import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Notification } from 'src/app/models/notify/notification';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

/**
 * This class represents an angular component that displauys a series of messages between users.
 * @export
 * @class ChatRootComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-root',
    templateUrl: './chat-root.component.html',
    styleUrls: ['./chat-root.component.scss']
})
export class ChatRootComponent implements OnInit, OnDestroy {

    private routeParametersSubscription?: Subscription;

    public options?: PrivateGroupConnectionOptions | ChannelConnectionOptions;

    public noChatFound = false;

    /**
     * Creates an instance of ChatRootComponent.
     * @memberof ChatRootComponent
     */
    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatRootComponent
     */
    public ngOnInit() {
        this.routeParametersSubscription = this
            .activatedRoute
            .paramMap
            .subscribe(params => {
                const id = +params.get('id');
                const isPrivateChat = this
                    .router
                    .url
                    .includes('private');

                if (isPrivateChat) {
                    this.options = new PrivateGroupConnectionOptions();
                    this.options.privateGroupId = id;
                    this.chatService.addSelfToPrivateGroup(id);
                    return;
                }

                const isChannelChat = this
                    .router
                    .url
                    .includes('channel');

                if (isChannelChat) {
                    this.options = new ChannelConnectionOptions();
                    this.options.channelId = id;
                    this.chatService.addSelfToChannel(id);
                    return;
                }

                this.noChatFound = true;

                const notification: Notification = {
                    title: 'No active chat found',
                    content: 'Consider joining a channel or start a new private chat!'
                };
                this.notifyService
                    .notify(notification);
           });
    }

    /**
     * Executes any necessary code for the destruction of the component.
     * @memberof ChatRootComponent
     */
    public ngOnDestroy() {
        if (this.routeParametersSubscription) {
            this.routeParametersSubscription
                .unsubscribe();
        }
    }

}
