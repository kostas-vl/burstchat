import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
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

    public display = this.displaySource.asObservable();

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
}
