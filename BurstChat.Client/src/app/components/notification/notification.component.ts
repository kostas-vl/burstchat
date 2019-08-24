import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

/**
 * This class represents an angular component that displays a notification to the user.
 * @class NotificationComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss']
})
export class NotificationComponent implements OnInit {

    @Input()
    public notification?: Notification;

    @Output()
    public onClose: EventEmitter<{}> = new EventEmitter();

    /**
     * Creates an instance of NotificationComponent.
     * @memberof NotificationComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof NotificationComponent
     */
    public ngOnInit(): void { }

    /**
     * Handles the close button click event.
     * @memberof NotificationComponent
     */
    public close(): void {
        this.onClose.emit();
    }

}