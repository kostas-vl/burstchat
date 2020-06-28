import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { WebSocketInterface, UA, debug, RTCSession } from 'jssip';
import { environment } from 'src/environments/environment';
import { RTCSessionContainer } from 'src/app/models/chat/rtc-session-container';
import { SipCredentials } from 'src/app/models/user/sip-credentials';
import { NotifyService } from 'src/app/services/notify/notify.service';

/**
 * This class represents an angular service that enables real time communication between burstchat
 * users, through web rtc.
 * @class RtcSessionService
 */
@Injectable()
export class RtcSessionService {

    private audio = new Audio();

    private socket = new WebSocketInterface(`ws://${environment.asteriskUrl}/ws`);

    private creds?: SipCredentials;

    private userAgentConfig?: any;

    private userAgent?: UA;

    private callConfig: any = {
        mediaConstraints: {
            audio: true,
            video: false
        },
        pcConfig: {
            iceServers: [
                {
                    urls: ['stun:stun.l.google.com:19302']
                }
            ]
        }
    };

    private incomingSession = new BehaviorSubject<RTCSessionContainer | null>(null);

    private session = new BehaviorSubject<RTCSessionContainer | null>(null);

    public onIncomingSession = this.incomingSession.asObservable();

    public onSession = this.session.asObservable();

    /**
     * Creates a new instance of RtcSessionService.
     * @memberof RtcSessionService
     */
    constructor(
        private httpClient: HttpClient,
        private notifyService: NotifyService
    ) {
        debug.enable('JsSIP:*');

        this.httpClient
            .get<SipCredentials>('/api/user/sip')
            .subscribe(creds => {
                this.creds = creds;
                this.userAgentConfig = {
                    sockets: [this.socket],
                    uri: `sip:${this.creds.username}@${environment.asteriskUrl}`,
                    password: this.creds.password,
                    register: true
                };
                this.userAgent = new UA(this.userAgentConfig);
                this.registerUserAgentEvent();
                this.userAgent.start();
            });
    }

    /**
     * Binds all neccessary user agent events to the appropriate callbacks.
     * @memberof RtcSessionService
     */
    private registerUserAgentEvent() {
        this.userAgent.on('connected', () => this.userAgentConnected());
        this.userAgent.on('registrationFailed', cause => this.userAgentRegistrationFailed(cause));
        this.userAgent.on('newRTCSession', event => this.userAgentNewSession(event));
    }

    /**
     * Handles the user agent connected event.
     * @memberof RtcSessionService
     */
    private userAgentConnected() {
        console.log('Sip user agent connected');
    }

    /**
     * Handles the user anget registration failed event.
     * @memberof RtcSessionService
     */
    private userAgentRegistrationFailed(cause) {
        console.warn('Registration failed with: ');
        console.warn(cause);
    }

    /**
     * Handles the user agent new rtc session event.
     * @memberof RtcSessionService
     */
    private userAgentNewSession(event) {
        console.log('New incoming RTC session');
        const session = new RTCSessionContainer(event.session);

        if (event.originator !== 'local') {
            this.registerSessionEvents(session);
            this.incomingSession.next(session);
        }
    }

    /**
     * Binds all neccessary events for the provided session, to the appropriate callbacks.
     * @param {RTCSessionContainer} session The session container instance.
     * @memberof RtcSessionService
     */
    private registerSessionEvents(session: RTCSessionContainer) {
        session
            .connecting
            .subscribe(args => this.sessionConnecting(args[0], args[1]));

        session
            .connected
            .subscribe(args => {
                session
                    .source
                    .connection
                    .AddEventListener('addstream', event => this.sessionAddStream(event));
            });

        session
            .progress
            .subscribe(args => this.sessionProgress(args[0], args[1]));

        session
            .confirmed
            .subscribe(args => this.sessionConfirmed(args[0], args[1]));

        session
            .failed
            .subscribe(args => this.sessionFailed(args[0], args[1]));

        session
            .ended
            .subscribe(args => this.sessionEnded(args[0], args[1]));
    }

    /**
     * Handles the session connecting event.
     * @param {RTCSession} source The session source instance.
     * @param {any} event The event args.
     * @memberof RtcSessionService
     */
    private sessionConnecting(source: RTCSession, event: any) {
        console.log(event);
    }

    /**
     * Handles the session progress event.
     * @param {RTCSession} source The session source instance.
     * @param {any} event The event arguments.
     * @memberof RtcSessionService
     */
    private sessionProgress(source: RTCSession, event: any) {
        console.log(event);
    }

    /**
     * Handles the session confirmed event.
     * @param {RTCSession} source The session source instance.
     * @param {any} event The event arguments.
     * @memberof RtcSessionService
     */
    private sessionConfirmed(source: RTCSession, event: any) {
        console.log(event);
    }

    /**
     * Handles the session failed event.
     * @param {RTCSession} source The session source instance.
     * @param {any} event The event arguments.
     * @memberof RtcSessionService
     */
    private sessionFailed(source: RTCSession, event: any) {
        console.warn('RTC session failed with cause: ');
        console.warn(event);
        this.notifyService.popupWarning('Call failed', 'Please try executing the call again in a few minutes.');

        let incomingSession = this.incomingSession.getValue();
        if (incomingSession?.source === source) {
            incomingSession = null;
            this.incomingSession.next(null);
            return;
        }

        let session = this.session.getValue();
        if (session?.source === source) {
            session = null;
            this.session.next(null);
            return;
        }
    }

    /**
     * Handles the session ended event.
     * @param {RTCSession} source The session source instance.
     * @param {any} event The event arguments.
     * @memberof RtcSessionService
     */
    private sessionEnded(source: RTCSession, event: any) {
        this.notifyService.popupInfo('Call ended');

        let incomingSession = this.incomingSession.getValue();
        if (incomingSession?.source === source) {
            incomingSession = null;
            this.incomingSession.next(null);
            return;
        }

        let session = this.session.getValue();
        if (session?.source === source) {
            session = null;
            this.session.next(null);
            return;
        }
    }

    /**
     * Handles the session's connection add stream event.
     * @param event The add stream event args.
     * @memberof RtcSessionService
     */
    private sessionAddStream(event) {
        this.audio.srcObject = event.stream;
        this.audio.play();
    }

    /**
     * Dials the provided sip user.
     * @param {number} sip The sip address of the target user.
     * @memberof RtcSessionService
     */
    public call(sip: number) {
        if (this.userAgent) {
            const session = this
                .userAgent
                .call(`sip:${sip}@${environment.asteriskUrl}`, this.callConfig);
            const container = new RTCSessionContainer(session);
            this.registerSessionEvents(container);
            this.session.next(container);
        }
    }

    /**
     * Answers the current incoming session.
     * @memberof RtcSessionService
     */
    public answer() {
        const session = this.incomingSession.getValue();
        if (session) {
            session.source.answer(this.callConfig);
            this.session.next(session);
            this.incomingSession.next(null);
        }
    }

    /**
     * Rejects an incoming call.
     * @memberof RtcSessionService
     */
    public reject() {
        const session = this.incomingSession.getValue();
        if (session) {
            session.source.terminate({
                status_code: 300,
                reason_phrase: 'reject'
            });
            this.incomingSession.next(null);
        }
    }

    /**
     * Hangs up the current session.
     * @memberof RtcSessionService
     */
    public hangup() {
        const session = this.session.getValue();
        if (session) {
            session.source.terminate({
                status_code: 300,
                reason_phrase: 'hang up'
            });
            this.session.next(null);
        }
    }

}
