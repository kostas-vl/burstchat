import { Injectable } from '@angular/core';
import { BurstChatError } from 'src/app/models/errors/error';

/**
 * This class represents an angular service that transmits signals to the notidy component
 * about new notifications that need to be displayed.
 * @class NotifyService
 */
@Injectable()
export class NotifyService {

    private get canDisplay() {
        return 'Notification' in window
            && Notification.permission !== 'denied'
            && !document.hasFocus();
    }

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

}
