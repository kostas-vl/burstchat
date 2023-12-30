import { Component, OnInit, OnDestroy } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { UserService } from 'src/app/services/user/user.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { ImageCropComponent } from 'src/app/components/shared/image-crop/image-crop.component';

/**
 * This class represents an angular component that displays to the user his current info for editing.
 * @export
 * @class EditUserInfoComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-edit-user-info',
    templateUrl: './edit-user-info.component.html',
    styleUrl: './edit-user-info.component.scss',
    standalone: true,
    imports: [
        DatePipe,
        DialogComponent,
        AvatarComponent,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        ImageCropComponent
    ]
})
export class EditUserInfoComponent implements OnInit, OnDestroy {

    private userSub?: Subscription;

    public user?: User;

    public changeAvatarVisible = false;

    public newAvatar?: string;

    /**
     * Creates a new instance of EditUserInfoComponent
     * @memberof EditUserInfoComponent
     */
    constructor(
        private userService: UserService,
        private chatService: ChatService
    ) { }

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
     * Handles the image crop event.
     * @param {string} event The event args.
     * @memberof EditUserInfoComponent
     */
    public onImageCropped(event: string) {
        this.newAvatar = event;
    }

    /**
     * Handles the change picture dialog's save button click event.
     * @memberof EditUserInfoComponent
     */
    public onSaveNewAvatar() {
        const user = { ...this.user, avatar: this.newAvatar };
        this.userService
            .update(user)
            .subscribe(_ => this.chatService.updateMyInfo());
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
