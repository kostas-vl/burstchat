import { NgClass } from '@angular/common';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCircle } from '@fortawesome/free-solid-svg-icons';
import { User } from 'src/app/models/user/user';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';

/**
 * This class represents an angular component that displays information about a subscribed user
 * of a server.
 * @export
 * @class UserComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-user',
    templateUrl: './user.component.html',
    styleUrl: './user.component.scss',
    standalone: true,
    imports: [NgClass, FontAwesomeModule, AvatarComponent]
})
export class UserComponent implements OnInit, OnDestroy {

    private userSubscription?: Subscription;

    private currentUser?: User;

    public circle = faCircle;

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
        if (this.currentUser && this.currentUser.id !== this.user.id) {
            this.router.navigate(['/core/chat/direct'], {
                queryParams: {
                    user: [this.currentUser.id, this.user.id]
                }
            });
        }
    }

}
