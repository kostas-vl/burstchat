import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { faCheck, faTimes, faUserCircle } from '@fortawesome/free-solid-svg-icons';
import { User } from 'src/app/models/user/user';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';

/**
 * This class represents an angular component that displays a dialog to the user about an incoming call.
 * @class
 * @implements {OnInit, OnDestroy}
 */
@Component({
    selector: 'burst-incoming-call',
    templateUrl: './incoming-call.component.html',
    styleUrls: ['./incoming-call.component.scss']
})
export class IncomingCallComponent implements OnInit, OnDestroy {

    private incomingSessionSub?: Subscription;

    private session?: RTCSessionContainer;

    public user?: User;

    public userIcon = faUserCircle;

    public answerIcon = faCheck;

    public ignoreIcon = faTimes;

    public dialogVisible = false;

    /**
     * Creates a new instance of IncomingCallComponent.
     * @memberof IncomingCallComponent
     */
    constructor(
        private router: Router,
        private userService: UserService,
        private rtcSessionService: RtcSessionService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof IncomingCallComponent
     */
    public ngOnInit() {
        this.incomingSessionSub = this
            .rtcSessionService
            .onIncomingSession$
            .subscribe(session => this.onNewSession(session));
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof IncomingCallComponent
     */
    public ngOnDestroy() {
        this.incomingSessionSub?.unsubscribe();
    }

    /**
     * Handles the new incoming rtc session.
     * @param {RTCSessionContainer} session The session object.
     * @memberof IncomingCallComponent
     */
    private onNewSession(session: RTCSessionContainer) {
        if (session) {
            this.session = session;
            const userId = +session.source.remote_identity.uri.user;
            this.userService
                .whoIs(userId)
                .subscribe(user => this.user = user);
            this.dialogVisible = true;
        } else {
            this.session = undefined;
            this.dialogVisible = false;
        }
    }

    /**
     * Handles the answer button click event.
     * @memberof IncomingCallComponent
     */
    public onAnswer() {
        this.rtcSessionService.answer();
        const first = this.user.id;
        const second = +this.session.source.remote_identity.uri.user;
        this.router.navigate(['/core/chat/direct'], {
            queryParams: {
                user: [first, second],
                display: 'call'
            }
        });
        this.dialogVisible = false;
    }

    /**
     * Handles the reject button click event.
     * @memberof IncomingCallComponent
     */
    public onReject() {
        this.rtcSessionService.reject();
        this.dialogVisible = false;
    }

}
