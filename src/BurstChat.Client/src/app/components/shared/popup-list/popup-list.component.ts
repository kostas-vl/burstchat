import { Component } from '@angular/core';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { PopupComponent } from 'src/app/components/shared/popup/popup.component';

/**
 * This class represents an angular component that displays a list of popup messages
 * on screen.
 * @export
 * @class PopupListComponent
 */
@Component({
    selector: 'burst-popup-list',
    templateUrl: './popup-list.component.html',
    styleUrl: './popup-list.component.scss',
    standalone: true,
    imports: [
        PopupComponent,
    ]
})
export class PopupListComponent {

    public popupMessages = this.notifyService.popupMessages;

    /**
     * Creates a new instance of PopupListComponent.
     * @memberof PopupListComponent
     */
    constructor(private notifyService: NotifyService) { }

    /**
     * Handles a popup components onClose event.
     * @param {number} index The index of the component from which the event
     *                       was emitted.
     * @memberof PopupListComponent
     */
    public onDismissPopup(index: number) {
        this.notifyService.dismissPopup(index);
    }

}
