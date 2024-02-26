import { Component, OnInit, OnDestroy, computed, effect, WritableSignal, signal, untracked } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCheck, faTimes, faUserCircle } from '@fortawesome/free-solid-svg-icons';
import { User } from 'src/app/models/user/user';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';
import { UserService } from 'src/app/services/user/user.service';
import { RtcSessionService } from 'src/app/services/rtc-session/rtc-session.service';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';

/**
 * This class represents an angular component that displays a dialog to the user about an incoming call.
 * @class
 * @implements {OnInit, OnDestroy}
 */
@Component({
    selector: 'burst-incoming-call',
    templateUrl: './incoming-call.component.html',
    styleUrl: './incoming-call.component.scss',
    standalone: true,
    imports: [
        FontAwesomeModule,
        DialogComponent,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        AvatarComponent
    ]
})
export class IncomingCallComponent {

    private session = computed(() => {
        const session = this.rtcSessionService.incomingSession();
        if (session) {
            const userId = +session.source.remote_identity.uri.user;
            untracked(() => {
                this.userService
                    .whoIs(userId)
                    .subscribe(user => this.user = user);
            })
            return session;
        } else {
            return undefined;
        }
    });

    public user?: User;

    public userIcon = faUserCircle;

    public answerIcon = faCheck;

    public ignoreIcon = faTimes;

    public dialogVisible = signal(false);

    /**
     * Creates a new instance of IncomingCallComponent.
     * @memberof IncomingCallComponent
     */
    constructor(
        private router: Router,
        private userService: UserService,
        private rtcSessionService: RtcSessionService
    ) {
        effect(() => {
            const session = this.rtcSessionService.incomingSession();
            this.dialogVisible.set(session ? true : false);
        }, { allowSignalWrites: true });
    }

    /**
     * Handles the answer button click event.
     * @memberof IncomingCallComponent
     */
    public onAnswer() {
        this.rtcSessionService.answer();
        const first = this.user.id;
        const second = +this.session().source.remote_identity.uri.user;
        this.router.navigate(['/core/chat/direct'], {
            queryParams: {
                user: [first, second],
                display: 'call'
            }
        });
        this.dialogVisible.set(false);
    }

    /**
     * Handles the reject button click event.
     * @memberof IncomingCallComponent
     */
    public onReject() {
        this.rtcSessionService.reject();
        this.dialogVisible.set(false);
    }

}
