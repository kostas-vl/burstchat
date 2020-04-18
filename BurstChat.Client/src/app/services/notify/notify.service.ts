import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { BurstChatError } from 'src/app/models/errors/error';
import { PopupMessage } from 'src/app/models/notify/popup-message';

/**
 * This class represents an angular service that transmits signals to the notidy component
 * about new notifications that need to be displayed.
 * @class NotifyService
 */
@Injectable()
export class NotifyService {

    private onPopupSource = new Subject<PopupMessage>();

    private get canDisplay() {
        return 'Notification' in window
            && Notification.permission !== 'denied'
            && !document.hasFocus();
    }

    public onPopup = this.onPopupSource.asObservable();

    /**
     * Creates an instance of NotifyService.
     * @memberof NotifyService
     */
    constructor() { }

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
    public popup(icon: any, title: string, content: string) {
        const message = new PopupMessage(icon, 'text', title, content);
        this.onPopupSource.next(message);
    }

    /**
     * This method will invoke a new popup message on screen, that will be displayed
     * as an error, based on the provided parameters.
     * @param {string} title The title of the popup.
     * @param {string} content The content of the popup.
     * @memberof NotifyService
     */
    public popupError(title: string, content: string) {
        const message = new PopupMessage(faTimes, 'text-danger', title, content);
        this.onPopupSource.next(message);
    }

}
