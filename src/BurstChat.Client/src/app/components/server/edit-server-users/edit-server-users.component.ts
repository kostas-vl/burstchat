import { Component, Input, Signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Server } from 'src/app/models/servers/server';
import { User } from 'src/app/models/user/user';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { UserService } from 'src/app/services/user/user.service';

/**
 * This class represents an angular component that enables invitings and editing server users.
 * @export
 * @class EditServerUsersComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-edit-server-users',
    templateUrl: './edit-server-users.component.html',
    styleUrl: './edit-server-users.component.scss',
    standalone: true,
    imports: [FormsModule]
})
export class EditServerUsersComponent {

    public newUserName = '';

    public users = computed(() => {
        const id = this.server().id.toString();
        return this.userService.usersCache[id] || [];
    });

    @Input({ required: true })
    public server: Signal<Server | null>;

    /**
     * Creates an instance of EditServerUsersComponent.
     * @memberof EditServerUsersComponent
     */
    constructor(
        private notifyService: NotifyService,
        private chatService: ChatService,
        private userService: UserService
    ) { }

    /**
     * Handles the send invite button click event.
     * @memberof EditServerComponent
     */
    public onInvite() {
        if (!this.newUserName) {
            const title = 'Could not send invitation';
            const content = 'Please provide a user name for the invitation!';
            this.notifyService.notify(title, content);
            return;
        }

        this.chatService.sendInvitation(this.server().id, this.newUserName);
        this.newUserName = '';
    }

    /**
     * Handles the delete user button click event.
     * @param {User} user The user instance to be deleted.
     * @memberof EditServerUsersComponent
     */
    public onDeleteUser(user: User) {
        if (user) {
            const subscription = this
                .server()
                .subscriptions
                .find(s => s.userId === user.id);
            if (subscription) {
                this.chatService.deleteSubscription(this.server().id, subscription);
            }
        }
    }

}
