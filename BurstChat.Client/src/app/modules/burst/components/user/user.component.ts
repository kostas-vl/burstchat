import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/models/user/user';

/**
 * This class represents an angular component that displays information about a subscribed user
 * of a server.
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

    @Input()
    public user?: User;

    @Input()
    public isActive = false;

    /**
     * Creates an instance of UserComponent.
     * @memberof UserComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof UserComponent
     */
    public ngOnInit() { }

}
