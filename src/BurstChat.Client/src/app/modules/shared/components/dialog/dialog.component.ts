import { Component, OnInit, Input, Output, EventEmitter, HostListener } from '@angular/core';

/**
 * This class represents an angular component that displays to the user a dialog.
 * @export
 * @class DialogComponent
 */
@Component({
    selector: 'burst-dialog',
    templateUrl: './dialog.component.html',
    styleUrls: ['./dialog.component.scss']
})
export class DialogComponent implements OnInit {

    public height = 0;

    public width = 0;

    public isVisible = false;

    @Output()
    public visibleChange = new EventEmitter<boolean>();

    @Input()
    public set visible(value: boolean) {
        this.isVisible = value;
    }

    /**
     * Creates a new instance of DialogComponent.
     * @memberof DialogComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof DialogComponent
     */
    public ngOnInit() {
        this.onWindowResize({ target: window });
    }

    /**
     * Listens and handles a window resize event.
     * @param {any} event The event arguments.
     * @memberof DialogComponent
     */
    @HostListener('window:resize', ['$event'])
    public onWindowResize(event: any) {
        this.height = event?.target?.innerHeight;
        this.width = event?.target?.innerWidth;
    }

}
