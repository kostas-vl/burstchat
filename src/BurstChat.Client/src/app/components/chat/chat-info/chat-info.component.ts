import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { DirectMessagingConnectionOptions } from 'src/app/models/chat/direct-messaging-connection-options';
import { UserService } from 'src/app/services/user/user.service';
import { RtcSessionService } from 'src/app/services/rtc-session/rtc-session.service';
import { UiLayerService } from 'src/app/services/ui-layer/ui-layer.service';

import {
    faCommentAlt,
    faLock,
    faComments,
    faPhone,
    faVideo,
    faQuestionCircle,
    faClone
} from '@fortawesome/free-solid-svg-icons';

/**
 * This class represents an angular component that displays on screen the top bar of the application
 * that provides various types of functionality and actions.
 * @export
 * @class ChatInfoComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-chat-info',
    templateUrl: './chat-info.component.html',
    styleUrl: './chat-info.component.scss',
    standalone: true,
    imports: [
        FormsModule,
        FontAwesomeModule
    ]
})
export class ChatInfoComponent {

    private internalOptions?: ChatConnectionOptions;

    public icon = undefined;

    public callIcon = faPhone;

    public videoIcon = faVideo;

    public helpIcon = faQuestionCircle;

    public switchIcon = faClone;

    public layoutState = this.uiLayerService.layout;

    public searchTerm: string | null = null;

    public get canCall() {
        return this.options instanceof DirectMessagingConnectionOptions
            && this.rtcSessionService.session() === null;
    }

    public get displayGoToCall() {
        if (this.options instanceof DirectMessagingConnectionOptions) {
            const first = this.options.directMessaging.firstParticipantUser;
            const second = this.options.directMessaging.secondParticipantUser;
            const session = this.rtcSessionService.session();
            const sessionUserId = +session?.source.remote_identity.uri.user;
            const isRightChat = sessionUserId === first.id || sessionUserId === second.id;
            return isRightChat
                && !this.canCall
                && this.layoutState() === 'chat';
        }
        return false;
    }

    public get displayGoToChat() {
        if (this.options instanceof DirectMessagingConnectionOptions) {
            const first = this.options.directMessaging.firstParticipantUser;
            const second = this.options.directMessaging.secondParticipantUser;
            const session = this.rtcSessionService.session();
            const sessionUserId = +session?.source.remote_identity.uri.user;
            const isRightChat = sessionUserId === first.id || sessionUserId === second.id;
            return isRightChat
                && !this.canCall
                && this.layoutState() === 'call';
        }
        return false;
    }

    public get options() {
        return this.internalOptions;
    }

    @Input()
    public set options(value: ChatConnectionOptions) {
        this.internalOptions = value;

        if (this.internalOptions instanceof ChannelConnectionOptions) {
            this.icon = faCommentAlt;
        } else if (this.internalOptions instanceof PrivateGroupConnectionOptions) {
            this.icon = faLock;
        } else if (this.internalOptions instanceof DirectMessagingConnectionOptions) {
            this.icon = faComments;
        } else {
            this.icon = undefined;
        }
    }

    /**
     * Creates a new instance of ChatInfoComponent.
     * @memberof ChatInfoComponent
     */
    constructor(
        private userService: UserService,
        private rtcSessionService: RtcSessionService,
        private uiLayerService: UiLayerService
    ) { }

    /**
     * Handles the call button click event.
     * @memberof ChatInfoComponent
     */
    public onCallClick() {
        if (this.options instanceof DirectMessagingConnectionOptions) {
            const dm = this.options.directMessaging;
            const user = this.userService.user();
            if (dm.firstParticipantUser.id !== user?.id) {
                this.rtcSessionService.call(dm.firstParticipantUser.id);
                this.uiLayerService.changeLayout('call');
                return;
            }

            if (dm.secondParticipantUser.id !== user?.id) {
                this.rtcSessionService.call(dm.secondParticipantUser.id);
                this.uiLayerService.changeLayout('call');
                return;
            }
        }
    }

    /**
     * Handles the click event of both the 'Go to call' and 'Go to chat' buttons.
     * @memberof ChatInfoComponent
     */
    public onChangeLayout(state: 'chat' | 'call') {
        this.uiLayerService.changeLayout(state);
    }

    /**
     * Handles the enter key event of the search field.
     * @memberof ChatInfoComponent
     */
    public onSearch() {
        this.uiLayerService.searchTerm(this.searchTerm);
    }

}
