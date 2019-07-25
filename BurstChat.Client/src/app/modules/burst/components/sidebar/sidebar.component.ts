import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that displays on screen the main sidebar of the application.
 * @export
 * @class SidebarComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {

    /**
     * Creates an instance of SidebarComponent.
     * @memberof SidebarComponent
     */
    constructor() { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof SidebarComponent
     */
    public ngOnInit() { }

}
