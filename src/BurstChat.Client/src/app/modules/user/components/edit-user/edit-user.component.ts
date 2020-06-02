import { Component, OnInit, OnDestroy } from '@angular/core';
import { User } from 'src/app/models/user/user';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { Subscription } from 'rxjs';

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
export class EditUserComponent implements OnInit, OnDestroy {

    private userSubscription?: Subscription;

    public user?: User;

    /**
     * Creates an instance of EditUserComponent.
     * @memberof EditUserComponent
     */
    constructor(private userService: UserService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditUserComponent
     */
    public ngOnInit() {
        this.userSubscription = this
            .userService
            .user
            .subscribe(user => {
                if (user) {
                    this.user = user;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditUserComponent
     */
    public ngOnDestroy() {
        if (this.userSubscription) {
            this.userSubscription
                .unsubscribe();
        }
    }

}
