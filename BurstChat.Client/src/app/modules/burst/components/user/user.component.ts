import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that displays on screen a user subscribed to the currently active
 * server.
 * @export
 * @class UserComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-user',
    templateUrl: './user.component.html',
    styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {

    /**
     * Creates an instance of UserComponent.
     * @memberof UserComponent
     */
    constructor() { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof UserComponent
     */
    public ngOnInit() { }

}
