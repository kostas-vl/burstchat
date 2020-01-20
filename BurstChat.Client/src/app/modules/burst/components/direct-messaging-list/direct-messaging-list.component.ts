import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { DirectMessagingService } from 'src/app/modules/burst/services/direct-messaging/direct-messaging.service';

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
export class DirectMessagingListComponent implements OnInit, OnDestroy {

    private usersSub?: Subscription;

    public users: User[];

    /**
     * Creates an instance of DirectMessagingListComponent.
     * @memberof DirectMessagingListComponent
     */
    constructor(private directMessagingService: DirectMessagingService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof DirectMessagingListComponent
     */
    public ngOnInit() {
        this.usersSub = this
            .directMessagingService
            .users
            .subscribe(users => {
                this.users = users;
                console.log(this.users);
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof DirectMessagingListComponent
     */
    public ngOnDestroy() {
        if (this.usersSub) {
            this.usersSub.unsubscribe();
        }
    }

}
