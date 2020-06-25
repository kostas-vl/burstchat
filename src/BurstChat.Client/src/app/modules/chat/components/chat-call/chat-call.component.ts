import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { faUserCircle, faPhoneSlash } from '@fortawesome/free-solid-svg-icons';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { DirectMessagingConnectionOptions } from 'src/app/models/chat/direct-messaging-connection-options';
import { User } from 'src/app/models/user/user';

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

    private sessionSub?: Subscription;

    private session?: any;

    public userIcon = faUserCircle;

    public hangupIcon = faPhoneSlash;

    public visible = false;

    public users: User[] = [];

    @Input()
    public options?: ChatConnectionOptions;

    /**
     * Creates a new instance of ChatCallComponent.
     * @memberof ChatCallComponent
     */
    constructor(private rtcSessionService: RtcSessionService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChatCallComponent
     */
    public ngOnInit() {
        this.sessionSub = this
            .rtcSessionService
            .onSession
            .subscribe(session => this.onNewSession(session));
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ChatCallComponent
     */
    public ngOnDestroy() {
        this.sessionSub?.unsubscribe();
    }

    /**
     * Executes any neccessary code for a new call session.
     * @private
     * @param {*} session The session call instance.
     * @memberof ChatCallComponent
     */
    private onNewSession(session: any) {
        if (!session) {
            this.session = undefined;
            this.visible = false;
            return;
        }

        this.session = session;

        if (this.options instanceof DirectMessagingConnectionOptions) {
            const first = this.options.directMessaging.firstParticipantUser;
            const second = this.options.directMessaging.secondParticipantUser;
            const sessionUser = +session.remote_identity.uri.user;
            if (sessionUser === first.id || sessionUser === second.id) {
                this.users = [first, second];
                this.visible = true;
                return;
            }
        }

        this.visible = false;
    }

}

