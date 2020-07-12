import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { UserService } from 'src/app/modules/burst/services/user/user.service';

/**
 * This class represents an angular component that displays to the user his current info for editing.
 * @export
 * @class EditUserInfoComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-edit-user-info',
    templateUrl: './edit-user-info.component.html',
    styleUrls: ['./edit-user-info.component.scss']
})
export class EditUserInfoComponent implements OnInit, OnDestroy {

    private userSub?: Subscription;

    public user?: User;

    public changeAvatarVisible = false;

    /**
     * Creates a new instance of EditUserInfoComponent
     * @memberof EditUserInfoComponent
     */
    constructor(private userService: UserService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditUserInfoComponent
     */
    public ngOnInit() {
        this.userSub = this
            .userService
            .user
            .subscribe(user => this.user = user);
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditUserInfoComponent
     */
    public ngOnDestroy() {
        this.userSub?.unsubscribe();
    }

    /**
     * Handles the change picture link click event.
     * @memberof EditUserInfoComponent
     */
    public onChangeAvatar() {
        this.changeAvatarVisible = true;
    }

    /**
     * Handles the change picture dialog's save button click event.
     * @memberof EditUserInfoComponent
     */
    public onSaveNewAvatar() {
        this.changeAvatarVisible = false;
    }

    /**
     * Handles the change picture dialog's cancel button click event.
     * @memberof EditUserInfoComponent
     */
    public onCancelNewAvatar() {
        this.changeAvatarVisible = false;
    }

}
