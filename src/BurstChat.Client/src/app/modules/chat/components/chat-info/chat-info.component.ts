import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { faCommentAlt, faLock, faComments, faPhone } from '@fortawesome/free-solid-svg-icons';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { DirectMessagingConnectionOptions } from 'src/app/models/chat/direct-messaging-connection-options';
import { User } from 'src/app/models/user/user';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';
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
export class ChatInfoComponent implements OnInit, OnDestroy {

    private userSub?: Subscription;

    private sessionSub?: Subscription;

    private user?: User;

    private session?: RTCSessionContainer;

    private internalOptions?: ChatConnectionOptions;

    public icon = undefined;

    public callIcon = faPhone;

    public canCall = false;

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

        this.updateCallStatus();
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
        this.sessionSub = this
            .rtcSessionService
            .onSession
            .subscribe(session => {
                this.session = session;
                this.updateCallStatus();
            });

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
        this.sessionSub?.unsubscribe();
        this.userSub?.unsubscribe();
    }

    /**
     * Makes the neccessary checks in order to display the call button to the
     * user or not.
     * @memberof ChatInfoComponent
     */
    private updateCallStatus() {
        if (this.options instanceof DirectMessagingConnectionOptions) {
            this.canCall = this.session
                ? false
                : true;
            return;
        }

        this.canCall = false;
    }

    /**
     * Handles the call button click event.
     * @memberof ChatInfoComponent
     */
    public onCallClick() {
        if (this.options instanceof DirectMessagingConnectionOptions) {
            const dm = this.options.directMessaging;
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
