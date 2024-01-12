import { Component, Input, WritableSignal, effect, signal, untracked } from '@angular/core';
import { Subscription } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPhoneSlash } from '@fortawesome/free-solid-svg-icons';
import { RtcSessionService } from 'src/app/services/rtc-session/rtc-session.service';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { DirectMessagingConnectionOptions } from 'src/app/models/chat/direct-messaging-connection-options';
import { User } from 'src/app/models/user/user';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';
import { UiLayerService } from 'src/app/services/ui-layer/ui-layer.service';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';

/**
 * This class represents an angular component that displays to the user information about a
 * current call.
 * @class ChatCallComponent
 * @implements {OnInit, OnDestroy}
 */
@Component({
    selector: 'burst-chat-call',
    templateUrl: './chat-call.component.html',
    styleUrl: './chat-call.component.scss',
    standalone: true,
    imports: [
        FontAwesomeModule,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        AvatarComponent
    ]
})
export class ChatCallComponent {

    private subscriptions: Subscription[] = [];

    private session: WritableSignal<RTCSessionContainer | null> = signal(null);

    public icons: any = {
        hangup: faPhoneSlash,
    }

    public users: User[] = [];

    public state: 'waiting' | 'confirmed' = 'waiting';

    @Input()
    public options?: ChatConnectionOptions;

    /**
     * Creates a new instance of ChatCallComponent.
     * @memberof ChatCallComponent
     */
    constructor(
        private rtcSessionService: RtcSessionService,
        private uiLayerService: UiLayerService
    ) {
        effect(() => {
            const session = this.rtcSessionService.session();
            if (session) {
                this.onNewSession(session);
                return;
            }
            this.reset();
        });

        effect(() => {
            const session = this.session();
            const sessionConfirmed = session.confirmed();
            if (sessionConfirmed && this.validSession(session)) {
                this.onSessionConfirmed();
            }
        });

        effect(() => {
            const session = this.session();
            const sessionFailed = session.failed();
            if (sessionFailed && this.validSession(session)) {
                this.onSessionFailed();
            }
        });
    }

    /**
     * Resets the values of specific properties to their intended original value.
     * @private
     * @memberof ChatCallComponent
     */
    private reset() {
        this.session = undefined;
        this.state = 'waiting';
    }

    /**
     * Checks whether the provided session instance is targeted towards the two users associated with the call.
     * @param {RTCSessionContainer} session The target rtc session instance.
     * @returns A boolean value indicating if the session user is either of the 2 users associated with the call.
     * @memberof ChatCallComponent
     */
    private validSession(session: RTCSessionContainer): boolean {
        if (session && this.options instanceof DirectMessagingConnectionOptions) {
            const first = this.options.directMessaging.firstParticipantUser;
            const second = this.options.directMessaging.secondParticipantUser;
            const sessionUser = +session.source.remote_identity.uri.user;
            return sessionUser === first.id || sessionUser === second.id;
        } else {
            return false;
        }
    }

    /**
     * Executes any neccessary code for a new call session.
     * @private
     * @param {RTCSessionContainer} session The session call instance.
     * @memberof ChatCallComponent
     */
    private onNewSession(session: RTCSessionContainer) {
        this.session.set(session);

        if (session && this.options instanceof DirectMessagingConnectionOptions) {
            const first = this.options.directMessaging.firstParticipantUser;
            const second = this.options.directMessaging.secondParticipantUser;
            const sessionUser = +this.session().source.remote_identity.uri.user;
            if (sessionUser === first.id || sessionUser === second.id) {
                this.users = [first, second];

                untracked(() => {
                    this.subscriptions[2] = this
                        .session()
                        .ended
                        .subscribe(_ => this.onSessionEnded());
                })

                if (session.source.isEstablished()) {
                    this.state = 'confirmed';
                }

                return;
            }
        }
    }

    /**
     * Handles the confirmed event of the active call session.
     * @private
     * @memberof ChatCallComponent
     */
    private onSessionConfirmed() {
        this.state = 'confirmed';
    }

    /**
     * Handles the ended event of the active call session.
     * @private
     * @memberof ChatCallComponent
     */
    private onSessionEnded() {
        this.uiLayerService.changeLayout('chat');
    }

    /**
     * Handles the failed event of the active call session.
     * @private
     * @memberof ChatCallComponent
     */
    private onSessionFailed() {
        this.uiLayerService.changeLayout('chat');
    }

    /**
     * Handles the hangup button click event.
     * @memberof ChatCallComponent
     */
    public onHangup() {
        this.rtcSessionService.hangup();
        this.uiLayerService.changeLayout('chat');
    }

}

