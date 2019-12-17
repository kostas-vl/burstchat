import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that displays the list of all direct messaging chats of the user.
 * @export
 * @class DirectMessagingListComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-direct-messaging-list',
    templateUrl: './direct-messaging-list.component.html',
    styleUrls: ['./direct-messaging-list.component.scss']
})
export class DirectMessagingListComponent implements OnInit {

    /**
     * Creates an instance of DirectMessagingListComponent.
     * @memberof DirectMessagingListComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof DirectMessagingListComponent
     */
    public ngOnInit() { }

}
