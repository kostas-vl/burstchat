import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that displays on screen a list of subscribed servers.
 * @export
 * @class ServerListComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-server-list',
    templateUrl: './server-list.component.html',
    styleUrls: ['./server-list.component.scss']
})
export class ServerListComponent implements OnInit {

    /**
     * Creates an instance of ServerListComponent.
     * @memberof ServerListComponent
     */
    constructor() { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ServerListComponent
     */
    public ngOnInit() { }

}
