import { Subject, Observable } from 'rxjs';
import { RTCSession }from 'jssip';

/**
 * This class encapsulates a jssip real time communication session.
 * @export
 * @class RTCSessionContainer
 */
export class RTCSessionContainer {

    private connectingSource = new Subject<[RTCSession, any]>();

    private connectedSource = new Subject<[RTCSession, any]>();

    private progressSource = new Subject<[RTCSession, any]>();

    private confirmedSource = new Subject<[RTCSession, any]>();

    private failedSource = new Subject<[RTCSession, any]>();

    private endedSource = new Subject<[RTCSession, any]>();

    public connecting = this.connectingSource.asObservable();

    public connected = this.connectedSource.asObservable();

    public progress = this.progressSource.asObservable();

    public confirmed = this.connectingSource.asObservable();

    public failed = this.failedSource.asObservable();

    public ended = this.endedSource.asObservable();

    /**
     * Creates a new instance of RTCSessionContainer.
     * @memberof RTCSessionContainer
     */
    constructor(public source: RTCSession) {
        this.source.on('connecting', event => this.onConnecting(event));
        this.source.on('connected', event => this.onConnected(event));
        this.source.on('progress', event => this.onProgress(event));
        this.source.on('confirmed', event => this.onConfirmed(event));
        this.source.on('failed', event => this.onFailed(event));
        this.source.on('ended', event => this.onEnded(event));
    }

    /**
     * Handles the rtc session connecting event.
     * @param {any} event The event arguments.
     * @memberof RTCSessionContainer
     */
    private onConnecting(event) {
        this.connectedSource.next([this.source, event]);
    }

    /**
     * Handles the rtc session connected event.
     * @param {any} event The event arguments.
     * @memberof RTCSessionContainer
     */
    private onConnected(event) {
        this.connectedSource.next([this.source, event]);
    }

    /**
     * Handles the rtc session progress event.
     * @param {any} event The event arguments.
     * @memberof RTCSessionContainer
     */
    private onProgress(event) {
        this.progressSource.next([this.source, event]);
    }

    /**
     * Handles the rtc session confirmed event.
     * @param {any} event The event arguments.
     * @memberof RTCSessionContainer
     */
    private onConfirmed(event) {
        this.confirmedSource.next([this.source, event]);
    }

    /**
     * Handles the rtc session failed event.
     * @param {any} event The event arguments.
     * @memberof RTCSessionContainer
     */
    private onFailed(event) {
        this.failedSource.next([this.source, event]);
    }

    /**
     * Handles the rtc session ended event.
     * @param {any} event The event arguments.
     * @memberof RTCSessionContainer
     */
    private onEnded(event) {
        this.endedSource.next([this.source, event]);
    }

}
