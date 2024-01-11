import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faDragon, faCog, faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { UserService } from 'src/app/services/user/user.service';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';

/**
 * This class represents an angular component that displays minor information about the user with
 * some actions.
 * @export
 * @class SidebarUserInfoComponent
 */
@Component({
    selector: 'burst-sidebar-user-info',
    templateUrl: './sidebar-user-info.component.html',
    styleUrl: './sidebar-user-info.component.scss',
    standalone: true,
    imports: [FontAwesomeModule, AvatarComponent]
})
export class SidebarUserInfoComponent {

    public dragon = faDragon;

    public cog = faCog;

    public signOut = faSignOutAlt;

    public user = this.userService.user;

    public get name() {
        return this.user?.name ?? '';
    }

    public get avatar() {
        return this.user()?.avatar ?? '';
    }

    /**
     * Creates a new instance of SidebarUserInfoComponent.
     * @memberof SidebarUserInfoComponent
     */
    constructor(
        private router: Router,
        private userService: UserService
    ) { }

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
