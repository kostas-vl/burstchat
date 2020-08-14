import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { PopupMessage } from 'src/app/models/notify/popup-message';
import { NotifyService } from 'src/app/services/notify/notify.service';

/**
 * This class represents an angular component that displays a list of popup messages
 * on screen.
 * @export
 * @class PopupListComponent
 */
@Component({
    selector: 'burst-popup-list',
    templateUrl: './popup-list.component.html',
    styleUrls: ['./popup-list.component.scss']
})
export class PopupListComponent implements OnInit, OnDestroy {

    private onPopupSub?: Subscription;

    public popupMessages: PopupMessage[] = [];

    /**
     * Creates a new instance of PopupListComponent.
     * @memberof PopupListComponent
     */
    constructor(private notifyService: NotifyService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof PopupListComponent
     */
    public ngOnInit() {
        this.onPopupSub = this
            .notifyService
            .onPopup
            .subscribe(message => {
                if (message) {
                    this.popupMessages.push(message);
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof PopupListComponent
     */
    public ngOnDestroy() {
        this.onPopupSub?.unsubscribe();
    }

    /**
     * Handles a popup components onClose event.
     * @param {number} index The index of the component from which the event
     *                       was emitted.
     * @memberof PopupListComponent
     */
    public onDismissPopup(index: number) {
        this.popupMessages.splice(index, 1);
    }

}
