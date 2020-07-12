import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component for dispaying and modifying user information.
 * @export
 * @class EditUserComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-edit-user',
    templateUrl: './edit-user.component.html',
    styleUrls: ['./edit-user.component.scss']
})
export class EditUserComponent implements OnInit {

    /**
     * Creates an instance of EditUserComponent.
     * @memberof EditUserComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditUserComponent
     */
    public ngOnInit() { }

}
