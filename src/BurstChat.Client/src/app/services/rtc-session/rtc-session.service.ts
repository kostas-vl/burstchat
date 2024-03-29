import { Injectable, WritableSignal, effect, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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

    private incomingSessionSource: WritableSignal<RTCSessionContainer | null> = signal(null);

    private sessionSource: WritableSignal<RTCSessionContainer | null> = signal(null);

    public incomingSession = this.incomingSessionSource.asReadonly();

    public session = this.sessionSource.asReadonly();

    /**
     * Creates a new instance of RtcSessionService.
     * @memberof RtcSessionService
     */
    constructor(
        private httpClient: HttpClient,
        private notifyService: NotifyService
    ) {
        debug.enable('JsSIP:*');

        effect(() => {
            const session = this.incomingSession();
            const sessionConnecting = session?.connecting();
            if (sessionConnecting) {
                const [source, event] = sessionConnecting;
                this.sessionConnecting(source, event);
            }
        });

        effect(() => {
            const session = this.incomingSession();
            const sessionConnected = session?.connected();
            if (sessionConnected) {
                session
                    .source
                    .connection
                    .ontrack = (event: any) => this.sessionAddStream(event);
            }
        });

        effect(() => {
            const session = this.incomingSession();
            const sessionProgress = session?.progress();
            if (sessionProgress) {
                const [source, event] = sessionProgress;
                this.sessionProgress(source, event);
            }
        });

        effect(() => {
            const session = this.incomingSession();
            const sessionConfirmed = session?.confirmed();
            if (sessionConfirmed) {
                const [source, event] = sessionConfirmed;
                this.sessionConfirmed(source, event);
            }
        });

        effect(() => {
            const session = this.incomingSession();
            const sessionFailed = session?.failed();
            if (sessionFailed) {
                const [source, event] = sessionFailed;
                this.sessionFailed(source, event);
            }
        });

        effect(() => {
            const session = this.session();
            const sessionEnded = session?.ended();
            if (sessionEnded) {
                const [source, event] = sessionEnded;
                this.sessionEnded(source, event);
            }
        });

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
        this.userAgent.on('registrationFailed', (cause: any) => this.userAgentRegistrationFailed(cause));
        this.userAgent.on('newRTCSession', (event: any) => this.userAgentNewSession(event));
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
    private userAgentRegistrationFailed(cause: any) {
        console.warn('Registration failed with: ');
        console.warn(cause);
    }

    /**
     * Handles the user agent new rtc session event.
     * @memberof RtcSessionService
     */
    private userAgentNewSession(event: any) {
        console.log('New incoming RTC session');
        const session = new RTCSessionContainer(event.session);

        if (event.originator !== 'local') {
            this.incomingSessionSource.set(session);
        }
    }

    /**
     * Handles the session connecting event.
     * @param {RTCSession} _source The session source instance.
     * @param {any} event The event args.
     * @memberof RtcSessionService
     */
    private sessionConnecting(_source: RTCSession, event: any) {
        console.log(event);
    }

    /**
     * Handles the session progress event.
     * @param {RTCSession} _source The session source instance.
     * @param {any} event The event arguments.
     * @memberof RtcSessionService
     */
    private sessionProgress(_source: RTCSession, event: any) {
        console.log(event);
    }

    /**
     * Handles the session confirmed event.
     * @param {RTCSession} _source The session source instance.
     * @param {any} event The event arguments.
     * @memberof RtcSessionService
     */
    private sessionConfirmed(_source: RTCSession, event: any) {
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
        this.notifyService.popupWarning('Call failed');

        let incomingSession = this.incomingSessionSource();
        if (incomingSession?.source === source) {
            incomingSession = null;
            this.incomingSessionSource.set(null);
            return;
        }

        let session = this.sessionSource();
        if (session?.source === source) {
            session = null;
            this.sessionSource.set(null);
            return;
        }
    }

    /**
     * Handles the session ended event.
     * @param {RTCSession} source The session source instance.
     * @param {any} _event The event arguments.
     * @memberof RtcSessionService
     */
    private sessionEnded(source: RTCSession, _event: any) {
        this.notifyService.popupInfo('Call ended');

        let incomingSession = this.incomingSessionSource();
        if (incomingSession?.source === source) {
            incomingSession = null;
            this.incomingSessionSource.set(null);
            return;
        }

        let session = this.sessionSource();
        if (session?.source === source) {
            session = null;
            this.sessionSource.set(null);
            return;
        }
    }

    /**
     * Handles the session's connection add stream event.
     * @param event The add stream event args.
     * @memberof RtcSessionService
     */
    private sessionAddStream(event: any) {
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
            this.sessionSource.set(container);
        }
    }

    /**
     * Answers the current incoming session.
     * @memberof RtcSessionService
     */
    public answer() {
        const session = this.incomingSessionSource();
        if (session) {
            session.source.answer(this.callConfig);
            this.sessionSource.set(session);
            this.incomingSessionSource.set(null);
        }
    }

    /**
     * Rejects an incoming call.
     * @memberof RtcSessionService
     */
    public reject() {
        const session = this.incomingSessionSource();
        if (session) {
            session.source.terminate({
                status_code: 300,
                reason_phrase: 'reject'
            });
            this.incomingSessionSource.set(null);
        }
    }

    /**
     * Hangs up the current session.
     * @memberof RtcSessionService
     */
    public hangup() {
        const session = this.sessionSource();
        if (session) {
            session.source.terminate({
                status_code: 300,
                reason_phrase: 'hang up'
            });
            this.sessionSource.set(null);
        }
    }

}
