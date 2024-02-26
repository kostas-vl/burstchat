import { NgClass } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCircle } from '@fortawesome/free-solid-svg-icons';
import { User } from 'src/app/models/user/user';
import { UserService } from 'src/app/services/user/user.service';
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
export class UserComponent {

    public circle = faCircle;

    @Input()
    public user?: User;

    @Input()
    public isActive = false;

    public get textColor() {
        const currentUser = this.userService.user();
        if (currentUser?.id === this.user.id) {
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
     * Handles the user select click event.
     * @memberof UserComponent
     */
    public onSelect() {
        const currentUser = this.userService.user();
        if (currentUser?.id !== this.user.id) {
            this.router.navigate(['/core/chat/direct'], {
                queryParams: {
                    user: [currentUser.id, this.user.id]
                }
            });
        }
    }

}
