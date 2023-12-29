import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';
import { faPhoneSlash, faExternalLinkAlt } from '@fortawesome/free-solid-svg-icons';

/**
 * This class represents an angular component that displays info and actions to the user for an ongoing
 * voice call.
 * @export
 * @class OngoingCallComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-ongoing-call',
    templateUrl: './ongoing-call.component.html',
    styleUrl: './ongoing-call.component.scss',
    standalone: true
})
export class OngoingCallComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    private user?: User;

    private session?: RTCSessionContainer;

    public visible = false;

    public chatRedirectIcon = faExternalLinkAlt;

    public hangupIcon = faPhoneSlash;

    /**
     * Creates an instance of OngoingCallComponent.
     * @memberof OngoingCallComponent
     */
    constructor(
        private router: Router,
        private userService: UserService,
        private rtcSessionService: RtcSessionService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof OngoingCallComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.userService
                .user
                .subscribe(user => this.user = user),

        this.rtcSessionService
            .onSession$
            .subscribe(session => {
                if (session) {
                    this.session = session;
                    this.visible = true;
                    return;
                }
                this.reset();
            })
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof OngoingCallComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }

    /**
     * Resets the values of specific properties to their intended original value.
     * @private
     * @memberof OngoingCallComponent
     */
    private reset() {
        this.session = undefined;
        this.visible = false;
    }

    /**
     * Handles the redirect button click event.
     * @memberof OngoingCallComponent
     */
    public onRedirectToChat() {
        if (this.user && this.session) {
            const first = this.user.id;
            const second = +this.session.source.remote_identity.uri.user;
            this.router.navigate(['/core/chat/direct'], {
                queryParams: {
                    user: [first, second]
                }
            });
        }
    }

    /**
     * Handles the hangup button click event.
     * @memberof OngoingCallComponent
     */
    public onHangup() {
        this.rtcSessionService.hangup();
    }

}
