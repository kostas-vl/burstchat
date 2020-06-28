import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { faVolumeUp, faMicrophone, faPhoneSlash, faMicrophoneSlash, faVolumeOff, faVolumeMute } from '@fortawesome/free-solid-svg-icons';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';

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

    private sessionSub?: Subscription;

    public session?: RTCSessionContainer;

    public volumeIcon = faVolumeUp;

    public microphoneIcon = faMicrophone;

    public hangupIcon = faPhoneSlash;

    public volumeActive = true;

    public microphoneActive = true;

    /**
     * Creates an instance of OngoingCallComponent.
     * @memberof OngoingCallComponent
     */
    constructor(private rtcSessionService: RtcSessionService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof OngoingCallComponent
     */
    public ngOnInit() {
        this.sessionSub = this
            .rtcSessionService
            .onSession
            .subscribe(session => this.session = session);
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof OngoingCallComponent
     */
    public ngOnDestroy() {
        this.sessionSub?.unsubscribe();
    }

    public onVolume() {
        this.volumeActive = !this.volumeActive;
        this.volumeIcon = this.volumeActive
            ? faVolumeUp
            : faVolumeMute;
    }

    public onMicrophone() {
        this.microphoneActive = !this.microphoneActive;
        this.microphoneIcon = this.microphoneActive
            ? faMicrophone
            : faMicrophoneSlash;
    }

    public onHangup() {
        this.rtcSessionService.hangup();
    }

}
