import { Injectable, WritableSignal, signal } from '@angular/core';
import { faBomb, faInfoCircle, faExclamationCircle, faCheck } from '@fortawesome/free-solid-svg-icons';
import { BurstChatError } from 'src/app/models/errors/error';
import { PopupMessage } from 'src/app/models/notify/popup-message';

/**
 * This class represents an angular service that transmits signals to the notidy component
 * about new notifications that need to be displayed.
 * @class NotifyService
 */
@Injectable({
    providedIn: 'root'
})
export class NotifyService {

    private popupMessagesSource: WritableSignal<PopupMessage[]> = signal([]);

    private get canDisplay() {
        return 'Notification' in window
            && Notification.permission !== 'denied'
            && !document.hasFocus();
    }

    public popupMessages = this.popupMessagesSource.asReadonly();

    /**
     * Creates an instance of NotifyService.
     * @memberof NotifyService
     */
    constructor() { }

    private pushNewMessage(value: PopupMessage[], message: PopupMessage): PopupMessage[] {
        value.push(message);
        return value;
    }

    /**
     * Requests permission from the user for displaying notifications if the user
     * has not already denied it.
     * @memberof NotifyService
     */
    public permission() {
        if (this.canDisplay) {
            Notification.requestPermission();
        }
    }

    /**
     * This method will invoke a new notification on screen based on the provided
     * data.
     * @memberof NotifyService
     * @param {Notification} data The notification data to be displayed.
     */
    public notify(title: string, content = ''): void {
        if (this.canDisplay) {
            const options: NotificationOptions = { body: content };
            const _ = new Notification(title, options);
        }
    }

    /**
     * This method will invoke a new notification on screen based on the provided
     * BurstChat error.
     * @memberof NotifyService
     * @param {BurstChatError | null} The error instance.
     */
    public notifyError(error: BurstChatError | null): void {
        if (error) {
            const title = 'An error occured';
            this.notify(title, error.message);
        }
    }

    /**
     * This method will invoke a new popup message on screen based on the provided
     * parameters.
     * @param {any} icon The icon of the popup.
     * @param {string} title The title of the popup.
     * @param {string} content The content of the popup.
     * @memberof NotifyService
     */
    public popup(icon: any, title: string, content = '') {
        const message = new PopupMessage(icon, 'text', title, content);
        this.popupMessagesSource.update(value => this.pushNewMessage(value, message));
    }

    /**
     * This method will invoke a new popup message on screen, that will be displayed
     * as a success message, based on the provided parameters.
     * @param {string} title The title of the popup.
     * @param {string} content The content of the popup.
     * @memberof NotifyService
     */
    public popupSuccess(title: string, content = '') {
        const message = new PopupMessage(faCheck, 'text-success', title, content)
        this.popupMessagesSource.update(value => this.pushNewMessage(value, message));
    }

    /**
     * This method will invoke a new popup message on screen, that will be displayed
     * as an informative message, based on the provided parameters.
     * @param {string} title The title of the popup.
     * @param {string} content The content of the popup.
     * @memberof NotifyService
     */
    public popupInfo(title: string, content = '') {
        const message = new PopupMessage(faInfoCircle, 'text-accent', title, content)
        this.popupMessagesSource.update(value => this.pushNewMessage(value, message));
    }

    /**
     * This method will invoke a new popup message on screen, that will be displayed
     * as a warning, based on the provided parameters.
     * @param {string} title The title of the popup.
     * @param {string} content The content of the popup.
     * @memberof NotifyService
     */
    public popupWarning(title: string, content = '') {
        const message = new PopupMessage(faExclamationCircle, 'text-warning', title, content);
        this.popupMessagesSource.update(value => this.pushNewMessage(value, message));
    }

    /**
     * This method will invoke a new popup message on screen, that will be displayed
     * as an error, based on the provided BurstChat error.
     * @param {BurstChatError | null} The error instance.
     * @memberof NotifyService
     */
    public popupError(error: BurstChatError | null): void {
        if (error) {
            const message = new PopupMessage(faBomb, 'text-danger', 'An error occured', error.message);
            this.popupMessagesSource.update(value => this.pushNewMessage(value, message));
        }
    }

    /**
     * This method will updated the internal messages signal in order to remove the message in the specified index.
     * @param {number} i The targer message index.
     * @memberof NotifyService
     */
    public dismissPopup(i: number): void {
        this.popupMessagesSource.update(value => {
            value.splice(i, 1);
            return value;
        })
    }
}
