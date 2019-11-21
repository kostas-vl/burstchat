import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { Notification } from 'src/app/models/notify/notification';
import { Server } from 'src/app/models/servers/server';
import { User } from 'src/app/models/user/user';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { UserService } from 'src/app/modules/burst/services/user/user.service';

/**
 * This class represents an angular component that enables invitings and editing server users.
 * @export
 * @class EditServerUsersComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-edit-server-users',
    templateUrl: './edit-server-users.component.html',
    styleUrls: ['./edit-server-users.component.scss']
})
export class EditServerUsersComponent implements OnInit, OnDestroy {

    private userCacheSub?: Subscription;

    public newUserName = '';

    public users: User[] = [];

    @Input()
    public server?: Server;

    /**
     * Creates an instance of EditServerUsersComponent.
     * @memberof EditServerUsersComponent
     */
    constructor(
        private notifyService: NotifyService,
        private serversService: ServersService,
        private chatService: ChatService,
        private userService: UserService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditServerUsersComponent
     */
    public ngOnInit() {
        this.userCacheSub = this
            .userService
            .usersCache
            .subscribe(cache => {
                const id = this.server.id.toString();
                this.users = cache[id] || [];
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditServerUsersComponent
     */
    public ngOnDestroy() {
        if (this.userCacheSub) {
            this.userCacheSub.unsubscribe();
        }
    }

    /**
     * Handles the send invite button click event.
     * @memberof EditServerComponent
     */
    public onInvite() {
        if (!this.newUserName) {
            const notification: Notification = {
                title: 'Could not send invitation',
                content: 'Please provide a user name for the invitation!'
            };
            this.notifyService.notify(notification);
            return;
        }

        // this.chatService.sendInvitation(this.server.id, 0);
        this.newUserName = '';
    }

    /**
     * Handles the delete user button click event.
     * @param {User} user The user instance to be deleted.
     * @memberof EditServerUsersComponent
     */
    public onDeleteUser(user: User) {
        if (user) {

        }
    }

}
