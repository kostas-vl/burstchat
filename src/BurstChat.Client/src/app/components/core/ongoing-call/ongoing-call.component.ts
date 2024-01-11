import { Component, computed } from '@angular/core';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPhoneSlash, faExternalLinkAlt } from '@fortawesome/free-solid-svg-icons';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';
import { UserService } from 'src/app/services/user/user.service';
import { RtcSessionService } from 'src/app/services/rtc-session/rtc-session.service';

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
    standalone: true,
    imports: [FontAwesomeModule]
})
export class OngoingCallComponent {

    private session?: RTCSessionContainer;

    public visible = computed(() => this.rtcSessionService.session() ? true : false);

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
     * Handles the redirect button click event.
     * @memberof OngoingCallComponent
     */
    public onRedirectToChat() {
        const user = this.userService.user();
        const session = this.rtcSessionService.session();
        if (user && this.session) {
            const first = user.id;
            const second = +session.source.remote_identity.uri.user;
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
