import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Notification } from 'src/app/models/notify/notification';
import { NotifyService } from 'src/app/services/notify/notify.service';

/**
 * This class represents an angular component that displays a list of notifications to the user.
 * @class NotificationListComponent
 * @implements {OnInit, OnDestroy}
 */
@Component({
  selector: 'app-notification-list',
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.scss']
})
export class NotificationListComponent implements OnInit, OnDestroy {

    public notifySubscription?: Subscription;

    public notifications: Notification[] = [];
    /**
     * Creates an instance of NotificationListComponent.
     * @memberof NotificationListComponent
     */
    constructor(private notifyService: NotifyService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof NotificationListComponent
     */
    public ngOnInit(): void {
        this.notifySubscription = this
            .notifyService
            .onNotify
            .subscribe(data => this.notifications.push(data));
    }

    /**
     * Executes any neccessary code for the destruction of component.
     * @memberof NotificationListComponent
     */
    public ngOnDestroy(): void {
        if (this.notifySubscription) {
            this.notifySubscription
                .unsubscribe();
        }
    }

    /**
     * Handles the onClose event of a notification component.
     * @memberof NotificationListComponent
     */
    public onDismissNotification(index: number): void {
        this.notifications.splice(index, 1);
    }

}
