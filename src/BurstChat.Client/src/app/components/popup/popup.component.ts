import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { PopupMessage } from 'src/app/models/notify/popup-message';

/**
 * This class represents an angular component that displays a popup message
 * on screen.
 * @export
 * @class PopupComponent
 */
@Component({
    selector: 'app-popup',
    templateUrl: './popup.component.html',
    styleUrls: ['./popup.component.scss']
})
export class PopupComponent implements OnInit {

    public closeIcon = faTimes;

    @Input()
    public message?: PopupMessage;

    @Output()
    public onClose: EventEmitter<{}> = new EventEmitter();

    /**
     * Creates a new instance of PopupComponent.
     * @memberof PopupComponent.
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof PopupComponent
     */
    public ngOnInit() { }

    /**
     * Handles the close button click event.
     * @memberof PopupComponent
     */
    public close() {
        this.onClose.emit();
    }

}
