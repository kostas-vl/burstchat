import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { UserService } from 'src/app/modules/burst/services/user/user.service';

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
export class UserComponent implements OnInit, OnDestroy {

    private userSubscription?: Subscription;

    private currentUser?: User;

    @Input()
    public user?: User;

    @Input()
    public isActive = false;

    public get textColor() {
        if (this.currentUser && this.currentUser.id === this.user.id) {
            return 'text-success';
        }

        return this.isActive
            ? 'text-accent-light'
            : '';
    }

    /**
     * Creates an instance of UserComponent.
     * @memberof UserComponent
     */
    constructor(
        private router: Router,
        private userService: UserService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof UserComponent
     */
    public ngOnInit() {
        this.userSubscription = this
            .userService
            .user
            .subscribe(currentUser => {
                if (currentUser) {
                    this.currentUser = currentUser;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof UserComponent
     */
    public ngOnDestroy() {
        if (this.userSubscription) {
            this.userSubscription
                .unsubscribe();
        }
    }

    /**
     * Handles the user select click event.
     * @memberof UserComponent
     */
    public onSelect() {
        // if (this.currentUser && this.currentUser.id !== this.user.id) {
        //     this.router.navigate(['/core/chat/private'], {
        //         queryParams: {
        //             user: [this.currentUser.id, this.user.id],
        //             name: `${this.currentUser.name}, ${this.user.name}`
        //         }
        //     });
        // }
    }

}
