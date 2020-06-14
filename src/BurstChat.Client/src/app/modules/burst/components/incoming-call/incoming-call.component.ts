import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { faCheck, faTimes } from '@fortawesome/free-solid-svg-icons';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';

/**
 * This class represents an angular component that displays a dialog to the user about an incoming call.
 * @class
 * @implements {OnInit, OnDestroy}
 */
@Component({
    selector: 'app-incoming-call',
    templateUrl: './incoming-call.component.html',
    styleUrls: ['./incoming-call.component.scss']
})
export class IncomingCallComponent implements OnInit, OnDestroy {

    private incomingSessionSub?: Subscription;

    public answerIcon = faCheck;

    public ignoreIcon = faTimes;

    public dialogVisible = false;

    /**
     * Creates a new instance of IncomingCallComponent.
     * @memberof IncomingCallComponent
     */
    constructor(private rtcSessionService: RtcSessionService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof IncomingCallComponent
     */
    public ngOnInit() {
        this.incomingSessionSub = this
            .rtcSessionService
            .onIncomingSession
            .subscribe(session => {
                if (session) {
                    this.dialogVisible = true;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof IncomingCallComponent
     */
    public ngOnDestroy() {
        this.incomingSessionSub?.unsubscribe();
    }

    /**
     * Handles the answer button click event.
     * @memberof IncomingCallComponent
     */
    public onAnswer() {
        this.rtcSessionService.answer();
        this.dialogVisible = false;
    }

    /**
     * Handles the ignore button click event.
     * @memberof IncomingCallComponent
     */
    public onIgnore() {
        this.dialogVisible = false;
    }

}
