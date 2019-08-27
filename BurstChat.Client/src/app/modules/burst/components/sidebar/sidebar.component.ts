import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { UserService } from 'src/app/modules/burst/services/user/user.service';

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
export class SidebarComponent implements OnInit, OnDestroy {

    private userSubscription?: Subscription;

    public user?: User;

    /**
     * Creates an instance of SidebarComponent.
     * @memberof SidebarComponent
     */
    constructor(private userService: UserService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof SidebarComponent
     */
    public ngOnInit() {
        this.userSubscription = this
            .userService
            .userObservable
            .subscribe(user => this.user = user);
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof SidebarComponent
     */
    public ngOnDestroy() {
        if (this.userSubscription) {
            this.userSubscription
                .unsubscribe();
        }
    }

}
