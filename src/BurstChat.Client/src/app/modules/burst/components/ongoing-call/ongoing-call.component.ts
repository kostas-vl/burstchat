import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';

import {
    faVolumeUp,
    faMicrophone,
    faPhoneSlash,
    faMicrophoneSlash,
    faVolumeOff,
    faVolumeMute,
    faExternalLinkAlt
} from '@fortawesome/free-solid-svg-icons';

/**
 * This class represents an angular component that displays info and actions to the user for an ongoing
 * voice call.
 * @export
 * @class OngoingCallComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-ongoing-call',
  templateUrl: './ongoing-call.component.html',
  styleUrls: ['./ongoing-call.component.scss']
})
export class OngoingCallComponent implements OnInit, OnDestroy {

    private userSub?: Subscription;

    private sessionSub?: Subscription;

    private user?: User;

    private session?: RTCSessionContainer;

    public visible = false;

    public chatRedirectIcon = faExternalLinkAlt;

    public volumeIcon = faVolumeUp;

    public microphoneIcon = faMicrophone;

    public hangupIcon = faPhoneSlash;

    public volumeActive = true;

    public microphoneActive = true;

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
        this.userSub = this
            .userService
            .user
            .subscribe(user => this.user = user);

        this.sessionSub = this
            .rtcSessionService
            .onSession
            .subscribe(session => {
                if (session) {
                    this.session = session;
                    this.visible = true;
                } else {
                    this.session = undefined;
                    this.visible = false;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof OngoingCallComponent
     */
    public ngOnDestroy() {
        this.userSub?.unsubscribe();
        this.sessionSub?.unsubscribe();
    }

    /**
     * Handles the redirect button click event.
     * @memberof OngoingCallComponent
     */
    public onRedirectToChat() {
        if (this.user && this.session) {
            var first = this.user.id;
            var second = +this.session.source.remote_identity.uri.user;
            console.log([first, second]);
            this.router.navigate(['/core/chat/direct'], {
                queryParams: {
                    user: [first, second]
                }
            });
        }
    }

    /**
     * Handles the volume button click event.
     * @memberof OngoingCallComponent
     */
    public onVolume() {
        this.volumeActive = !this.volumeActive;
        this.volumeIcon = this.volumeActive
            ? faVolumeUp
            : faVolumeMute;
    }

    /**
     * Handles the microphone button click event.
     * @memberof OngoingCallComponent
     */
    public onMicrophone() {
        this.microphoneActive = !this.microphoneActive;
        this.microphoneIcon = this.microphoneActive
            ? faMicrophone
            : faMicrophoneSlash;
    }

    /**
     * Handles the hangup button click event.
     * @memberof OngoingCallComponent
     */
    public onHangup() {
        this.rtcSessionService.hangup();
    }

}
