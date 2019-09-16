import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { UserService } from 'src/app/modules/burst/services/user/user.service';

/**
 * This class represents an angular component that displays minor information about the user with
 * some actions.
 * @export
 * @class SidebarUserInfoComponent
 */
@Component({
  selector: 'app-sidebar-user-info',
  templateUrl: './sidebar-user-info.component.html',
  styleUrls: ['./sidebar-user-info.component.scss']
})
export class SidebarUserInfoComponent implements OnInit, OnDestroy {

    private userSubscription?: Subscription;

    public user?: User;

    /**
     * Creates a new instance of SidebarUserInfoComponent.
     * @memberof SidebarUserInfoComponent
     */
    constructor(
        private router: Router,
        private userService: UserService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof SidebarUserInfoComponent
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
     * @memberof SidebarUserInfoComponent
     */
    public ngOnDestroy() {
        if (this.userSubscription) {
            this.userSubscription
                .unsubscribe();
        }
    }

    /**
     * Handles the edit user button click event.
     * @memberof SidebarUserInfoComponent
     */
    public onEditUser() {
        this.router.navigateByUrl('/core/user');
    }

    /*
     * Handles the logout button click event.
     * @memberof ChatInfoComponent
     */
    public onLogout(): void {
        this.router.navigateByUrl('/session/logout');
    }

}
