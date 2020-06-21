import { Component, OnInit, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { faCommentAlt, faLock, faComments, faPhone } from '@fortawesome/free-solid-svg-icons';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { DirectMessagingConnectionOptions } from 'src/app/models/chat/direct-messaging-connection-options';
import { User } from 'src/app/models/user/user';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';

/**
 * This class represents an angular component that displays on screen the top bar of the application
 * that provides various types of functionality and actions.
 * @export
 * @class ChatInfoComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-chat-info',
    templateUrl: './chat-info.component.html',
    styleUrls: ['./chat-info.component.scss']
})
export class ChatInfoComponent implements OnInit {

    private userSub?: Subscription;

    private user?: User;

    public optionsValue?: ChatConnectionOptions;

    public icon = undefined;

    public callIcon = faPhone;

    public canCall = false;

    @Input()
    public set options(value: ChatConnectionOptions) {
        this.optionsValue = value;

        if (this.optionsValue instanceof ChannelConnectionOptions) { 1
            this.icon = faCommentAlt;
            this.canCall = false;
        } else if (this.optionsValue instanceof PrivateGroupConnectionOptions) {
            this.icon = faLock;
            this.canCall = false;
        } else if (this.optionsValue instanceof DirectMessagingConnectionOptions) {
            this.icon = faComments;
            this.canCall = true;
        } else {
            this.icon = undefined;
            this.canCall = false;
        }
    }

    /**
     * Creates a new instance of ChatInfoComponent.
     * @memberof ChatInfoComponent
     */
    constructor(
        private userService: UserService,
        private rtcSessionService: RtcSessionService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChatInfoComponent
     */
    public ngOnInit() {
        this.userSub = this
            .userService
            .user
            .subscribe(user => this.user = user);
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChatInfoComponent
     */
    public ngOnDestroy() {
        this.userSub?.unsubscribe();
    }

    /**
     * Handles the call button click event.
     * @memberof ChatInfoComponent
     */
    public onCallClick() {
        if (this.optionsValue instanceof DirectMessagingConnectionOptions) {
            const options = this.optionsValue as DirectMessagingConnectionOptions;
            const dm = options.directMessaging;
            if (dm.firstParticipantUser.id !== this.user.id) {
                this.rtcSessionService.call(dm.firstParticipantUser.id);
                return;
            }

            if (dm.secondParticipantUser.id !== this.user.id) {
                this.rtcSessionService.call(dm.secondParticipantUser.id);
                return;
            }
        }
    }

}
