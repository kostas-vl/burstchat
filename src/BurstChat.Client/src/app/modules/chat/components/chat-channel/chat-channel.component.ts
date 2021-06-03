import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Message } from 'src/app/models/chat/message';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { UiLayerService } from 'src/app/modules/chat/services/ui-layer/ui-layer.service';

/**
 * This class represents an angular component that is the root component that contains a server channel chat.
 * @export
 * @class ChatChannelComponent
 * @implements {OnInit, OnDestroy}
 */
@Component({
    selector: 'burst-chat-channel',
    templateUrl: './chat-channel.component.html',
    styleUrls: ['./chat-channel.component.scss'],
    providers: [UiLayerService]
})
export class ChatChannelComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    private internalOptions?: ChannelConnectionOptions;

    public noChatFound = false;

    public layoutState: 'chat' | 'call' = 'chat';

    public get options() {
        return this.internalOptions;
    }

    public editMessageData: { visible: boolean, message?: Message } = { visible: false, message: null };

    public deleteMessageData: { visible: boolean, message?: Message} = { visible: false, message: null};

    /**
     * Creates an instance of ChatChannelComponent.
     * @memberof ChatChannelComponent
     */
    constructor(
        private activatedRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private chatService: ChatService,
        private uiLayerService: UiLayerService
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
                        this.internalOptions = new ChannelConnectionOptions();
                        this.internalOptions.signalGroup = `channel:${id}`;
                        this.internalOptions.name = name;
                        this.internalOptions.id = id;
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
            this.uiLayerService
                .toggleChatView$
                .subscribe(s => this.layoutState = s),
            this.uiLayerService
                .editMessage$
                .subscribe(message => this.editMessageData = {
                    visible: true,
                    message: message,
                }),
            this.uiLayerService
                .deleteMessage$
                .subscribe(message => this.deleteMessageData = {
                    visible: true,
                    message: message,
                })
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
