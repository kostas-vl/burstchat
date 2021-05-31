import { Component, EventEmitter, OnInit, Input, Output } from '@angular/core';

@Component({
    selector: 'burst-message-edit-dialog',
    templateUrl: './message-edit-dialog.component.html',
    styleUrls: ['./message-edit-dialog.component.scss']
})
export class MessageEditDialogComponent implements OnInit {

    public isVisible = false;

    @Input()
    public message?: string;

    @Input()
    public set visible(value: boolean) {
        this.isVisible = value;
    }

    /**
     * Creates a new instance of MessageEditDialogComponent.
     * @memberof MessageEditDialogComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof MessageEditDialogComponent
     */
    public ngOnInit() { }

}
