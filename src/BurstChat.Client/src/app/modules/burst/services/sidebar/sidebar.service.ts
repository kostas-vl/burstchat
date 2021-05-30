import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { DisplayDirectMessages } from 'src/app/models/sidebar/display-direct-messages';
import { DisplayServer } from 'src/app/models/sidebar/display-server';

/**
 * This class represents an angular service that exposes an API for displaying content on the sidebar.
 * @export
 * @class SidebarService
 */
@Injectable()
export class SidebarService {

    private displaySource = new BehaviorSubject<DisplayDirectMessages | DisplayServer>(new DisplayDirectMessages());

    private addServerDialogSource = new Subject<boolean>();

    public display = this.displaySource.asObservable();

    public addServerDialog = this.addServerDialogSource.asObservable();

    /**
     * Creates an instance of SidebarService.
     * @memberof SidebarService
     */
    constructor() { }

    /**
     * Activate the display of either a server or the direct messsages based on the provided options.
     * @param {(DisplayDirectMessages | DisplayServer)} options The options instance.
     * @memberof SidebarService
     */
    public toggleDisplay(options: DisplayDirectMessages | DisplayServer) {
        this.displaySource.next(options);
    }

    /**
     * Control the visibility of the add server dialog.
     * @param {boolean} visible The value indicating the visibility of the dialog.
     * @memberof SidebarService
     */
    public toggleAddServerDialog(visible: boolean) {
        this.addServerDialogSource.next(visible);
    }

}
