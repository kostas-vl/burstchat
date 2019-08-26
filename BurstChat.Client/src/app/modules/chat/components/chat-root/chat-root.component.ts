import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
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

    private routeParametersSubscription?: Subscription;

    public options?: PrivateGroupConnectionOptions | ChannelConnectionOptions;

    public noChatFound = false;

    /**
     * Creates an instance of ChatRootComponent.
     * @memberof ChatRootComponent
     */
    constructor(
        private activatedRoute: ActivatedRoute,
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
                    .activatedRoute
                    .outlet
                    .includes("private");

                if (isPrivateChat) {
                    this.options = new PrivateGroupConnectionOptions();
                    this.options.privateGroupId = id;
                    this.chatService.InitializeConnection();
                    return;
                }

                const isChannelChat = this
                    .activatedRoute
                    .outlet
                    .includes("channel");

                if (isChannelChat) {
                    this.options = new ChannelConnectionOptions();
                    this.options.channelId = id;
                    this.chatService.InitializeConnection();
                    return;
                }

                this.noChatFound = true;
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

        this.chatService
            .DisposeConnection();
    }

}
