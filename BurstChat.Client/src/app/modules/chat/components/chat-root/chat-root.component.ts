import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Notification } from 'src/app/models/notify/notification';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
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

    private onReconnectedSubscription?: Subscription;

    public options?: ChatConnectionOptions;

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
            .queryParamMap
            .subscribe(params => {
                const name = params.get('name');
                const id = +params.get('id');
                const currentUrl = this.router.url;
                this.assignOptions(currentUrl, name, id);
                this.assignSelfToChat(this.options);
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
     * Executes any necessary code for the destruction of the component.
     * @memberof ChatRootComponent
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

    /**
     * Checks the provided url and tries to assign a proper connection options value
     * to the options property.
     * @private
     * @param {string} currentUrl The current url of the application.
     * @param {string | null} name The name found in the parameters.
     * @param {number | null} id The id found in the parameters.
     * @memberof ChatRootComponent
     */
    private assignOptions(currentUrl: string, name: string | null, id: number | null) {
        const detailsProvided = name !== null && id !== null;
        const isPrivateChat = currentUrl.includes('private');

        if (isPrivateChat && detailsProvided) {
            this.options = new PrivateGroupConnectionOptions();
            this.options.signalGroup = `privateGroup:${id}`;
            this.options.name = name;
            this.options.id = id;
            return;
        }

        const isChannelChat = currentUrl.includes('channel');

        if (isChannelChat && detailsProvided) {
            this.options = new ChannelConnectionOptions();
            this.options.signalGroup = `channel:${id}`;
            this.options.name = name;
            this.options.id = id;
            return;
        }
    }

    /**
     * Attaches the application to the appropriate chat group in the signal server based on the
     * provided options.
     * @private
     * @param {ChatConnectionOptions} options The chat connection options.
     * @memberof ChatRootComponent
     */
    private assignSelfToChat(options: ChatConnectionOptions) {
        if (this.options) {
            this.chatService.addSelfToChat(this.options);
        } else {
            this.noChatFound = true;

            const notification: Notification = {
                title: 'No active chat found',
                content: 'Consider joining a channel or start a new private chat!'
            };
            this.notifyService.notify(notification);
        }
    }

}
