import { Component, OnInit, OnDestroy, effect } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Message } from 'src/app/models/chat/message';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { UiLayerService } from 'src/app/services/ui-layer/ui-layer.service';
import { ChatMessagesComponent } from 'src/app/components/chat/chat-messages/chat-messages.component';
import { ChatInfoComponent } from 'src/app/components/chat/chat-info/chat-info.component';
import { ChatInputComponent } from 'src/app/components/chat/chat-input/chat-input.component';
import { MessageEditDialogComponent } from 'src/app/components/chat/message-edit-dialog/message-edit-dialog.component';
import { MessageDeleteDialogComponent } from 'src/app/components/chat/message-delete-dialog/message-delete-dialog.component';

/**
 * This class represents an angular component that is the root component that contains a server channel chat.
 * @export
 * @class ChatChannelComponent
 * @implements {OnInit, OnDestroy}
 */
@Component({
    selector: 'burst-chat-channel',
    templateUrl: './chat-channel.component.html',
    styleUrl: './chat-channel.component.scss',
    standalone: true,
    imports: [
        ChatMessagesComponent,
        ChatInfoComponent,
        ChatInputComponent,
        MessageEditDialogComponent,
        MessageDeleteDialogComponent
    ],
    providers: [UiLayerService]
})
export class ChatChannelComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    private internalOptions?: ChannelConnectionOptions;

    public chatFound = true;

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
    ) {
        effect(() => {
            if (this.chatService.onReconnected() && this.options) {
                this.chatService.addSelfToChat(this.options);
            }
        });
    }

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
                        this.chatFound = false;
                        const title = 'No active chat found';
                        const content = 'Consider joining a channel or start a new private chat!';
                        this.notifyService.notify(title, content);
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
