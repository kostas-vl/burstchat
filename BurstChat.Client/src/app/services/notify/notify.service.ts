import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { BurstChatError } from 'src/app/models/errors/error';
import { Notification } from 'src/app/models/notify/notification';

/**
 * This class represents an angular service that transmits signals to the notidy component
 * about new notifications that need to be displayed.
 * @class NotifyService
 */
@Injectable()
export class NotifyService {

  private notifySource = new Subject<Notification>();

    public onNotify = this.notifySource.asObservable();

    /**
     * Creates an instance of NotifyService.
     * @memberof NotifyService
     */
    constructor() { }

    /**
     * This method will invoke a new notification on screen based on the provided
     * data.
     * @memberof NotifyService
     * @param {Notification} data The notification data to be displayed.
     */
    public notify(data: Notification): void {
        if (data) {
            this.notifySource
                .next(data);
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
            const notification: Notification = {
                title: 'An error occured',
                content: error.message
            };
            this.notify(notification);
        }
    }

}
