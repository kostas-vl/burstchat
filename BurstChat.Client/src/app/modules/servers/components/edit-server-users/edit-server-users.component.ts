import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Notification } from 'src/app/models/notify/notification';
import { Server } from 'src/app/models/servers/server';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

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

    private activeServerSubscription?: Subscription;

    public server?: Server;

    public newUserId = '';

    /**
     * Creates an instance of EditServerUsersComponent.
     * @memberof EditServerUsersComponent
     */
    constructor(
        private notifyService: NotifyService,
        private serversService: ServersService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditServerUsersComponent
     */
    public ngOnInit() {
        this.activeServerSubscription = this
            .serversService
            .activeServer
            .subscribe(server => {
                if (server) {
                    this.server = server;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditServerUsersComponent
     */
    public ngOnDestroy() {
        if (this.activeServerSubscription) {
            this.activeServerSubscription
                .unsubscribe();
        }
    }


    /**
     * Handles the send invite button click event.
     * @memberof EditServerComponent
     */
    public onSendInvite() {
        if (!this.newUserId) {
            const notification: Notification = {
                title: 'Could not send invitation',
                content: 'Please provide a user id for the invitation!'
            };
            this.notifyService.notify(notification);
            return;
        }

        this.chatService.sendInvitation(this.server.id, +this.newUserId);
        this.newUserId = '';
    }

}
