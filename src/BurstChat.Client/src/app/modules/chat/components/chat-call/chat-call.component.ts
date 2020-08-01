import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { DirectMessagingConnectionOptions } from 'src/app/models/chat/direct-messaging-connection-options';
import { User } from 'src/app/models/user/user';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';
import { ChatLayoutService } from 'src/app/modules/chat/services/chat-layout/chat-layout.service';

import {
    faUserCircle,
    faPhoneSlash,
} from '@fortawesome/free-solid-svg-icons';

/**
 * This class represents an angular component that displays to the user information about a
 * current call.
 * @class ChatCallComponent
 * @implements {OnInit, OnDestroy}
 */
@Component({
    selector: 'app-chat-call',
    templateUrl: './chat-call.component.html',
    styleUrls: ['./chat-call.component.scss']
})
export class ChatCallComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    private session?: RTCSessionContainer;

    private internalOptions?: ChatConnectionOptions;

    public userIcon = faUserCircle;

    public hangupIcon = faPhoneSlash;

    public visible = false;

    public sessionConfirmed = false;

    public users: User[] = [];

    public get options() {
        return this.internalOptions;
    }

    @Input()
    public set options(value: ChatConnectionOptions) {
        this.internalOptions = value;
    }

    /**
     * Creates a new instance of ChatCallComponent.
     * @memberof ChatCallComponent
     */
    constructor(
        private rtcSessionService: RtcSessionService,
        private chatLayoutService: ChatLayoutService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChatCallComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.rtcSessionService
                .onSession
                .subscribe(session => {
                    if (session) {
                        this.onNewSession(session);
                        return;
                    }
                    this.reset();
                })
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChatCallComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }


    /**
     * Resets the values of specific properties to their intended original value.
     * @private
     * @memberof ChatCallComponent
     */
    private reset() {
        this.session = undefined;
        this.visible = false;
        this.sessionConfirmed = false;
    }

    /**
     * Executes any neccessary code for a new call session.
     * @private
     * @param {RTCSessionContainer} session The session call instance.
     * @memberof ChatCallComponent
     */
    private onNewSession(session: RTCSessionContainer) {
        this.session = session;

        if (this.session && this.options instanceof DirectMessagingConnectionOptions) {
            const first = this.options.directMessaging.firstParticipantUser;
            const second = this.options.directMessaging.secondParticipantUser;
            const sessionUser = +this.session.source.remote_identity.uri.user;
            if (sessionUser === first.id || sessionUser === second.id) {
                this.users = [first, second];
                this.subscriptions[1] = this
                    .session
                    .confirmed
                    .subscribe(_ => this.sessionConfirmed = true);
                this.subscriptions[2] = this
                    .session
                    .ended
                    .subscribe(_ => this.chatLayoutService.toggle('chat'));
                return;
            }
        }
    }

    /**
     * Handles the hangup button click event.
     * @memberof ChatCallComponent
     */
    public onHangup() {
        this.rtcSessionService.hangup();
        this.chatLayoutService.toggle('chat');
    }

}

