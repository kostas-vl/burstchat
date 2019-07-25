import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that displays a list of users subscribed to the currently active
 * server.
 * @export
 * @class UserListComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {

    /**
     * Creates an instance of UserListComponent.
     * @memberof UserListComponent
     */
    constructor() { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof UserListComponent
     */
    public ngOnInit() { }

}
