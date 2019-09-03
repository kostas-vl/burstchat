import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { Message } from 'src/app/models/chat/message';
import { User } from 'src/app/models/user/user';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { StorageService } from 'src/app/services/storage/storage.service';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { ChatService } from 'src/app/modules/chat/services/chat-service/chat.service';

/**
 * This class represents an angular component that displays an input to which a user that send
 * a new message.
 * @export
 * @class ChatInputComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-input',
    templateUrl: './chat-input.component.html',
    styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent implements OnInit, OnDestroy {

    private userSubscription?: Subscription;

    private user?: User;

    public inputContent?: string;

    @Input()
    public options?: PrivateGroupConnectionOptions | ChannelConnectionOptions;

    /**
     * Creates an instance of ChatInputComponent.
     * @memberof ChatInputComponent
     */
    constructor(
        private storageService: StorageService,
        private userService: UserService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChatInputComponent
     */
    public ngOnInit() {
        this.userSubscription = this
            .userService
            .userObservable
            .subscribe(user => {
                if (user) {
                    this.user = user;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChatInputComponent
     */
    public ngOnDestroy() {
        if (this.userSubscription) {
            this.userSubscription
                .unsubscribe();
        }
    }

    /**
     * When the enter key is pressed in the message box and the input content has a value a new message is sent
     * to the connected signal server.
     * @memberof ChatInputComponent
     */
    public onEnterKeyPressed() {
        if (this.user && this.inputContent) {
            const message: Message = {
                id: 0,
                userId: this.user.id,
                user: this.user,
                content: this.inputContent,
                datePosted: new Date(),
                edited: false
            };

            if (this.options instanceof PrivateGroupConnectionOptions) {
                const groupId = this.options.privateGroupId;
                this.chatService.postPrivateGroupMessage(groupId, message);
                this.inputContent = undefined;
                return;
            }

            if (this.options instanceof ChannelConnectionOptions) {
                const channelId  = this.options.channelId;
                this.chatService.postChannelMessage(channelId, message);
                this.inputContent = undefined;
                return;
            }
        }
    }

}
