import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Invitation } from 'src/app/models/servers/invitation';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SidebarService } from 'src/app/modules/burst/services/sidebar/sidebar.service';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { ChannelsService } from 'src/app/modules/burst/services/channels/channels.service';
import { DirectMessagingService } from 'src/app/modules/burst/services/direct-messaging/direct-messaging.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { MediaService } from 'src/app/modules/burst/services/media/media.service';
import { RtcSessionService } from 'src/app/modules/burst/services/rtc-session/rtc-session.service';

/**
 * This class represents an angular component that displays on screen all components of
 * the burst chat client.
 * @class LayoutComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss'],
    providers: [
        SidebarService,
        UserService,
        ServersService,
        ChannelsService,
        DirectMessagingService,
        ChatService,
        MediaService,
        RtcSessionService
    ]
})
export class LayoutComponent implements OnInit, OnDestroy {

    private onConnectedSub?: Subscription;

    private onReconnectedSub?: Subscription;

    private invitationsSub?: Subscription;

    private newInvitationSub?: Subscription;

    private userUpdatedSub?: Subscription;

    public loading = true;

    /**
     * Creates an instance of LayoutComponent.
     * @memberof LayoutComponent
     */
    constructor(
        private notifyService: NotifyService,
        private userService: UserService,
        private directMessagingService: DirectMessagingService,
        private chatService: ChatService,
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof LayoutComponent
     */
    public ngOnInit() {
        this.onConnectedSub = this
            .chatService
            .onConnected
            .subscribe(() => {
                setTimeout(() => {
                    this.chatService.getInvitations();
                    this.loading = false;
                }, 300);
            });

        this.onReconnectedSub = this
            .chatService
            .onReconnected
            .subscribe(() => {
                this.chatService.getInvitations();
            });

        this.invitationsSub = this
            .chatService
            .invitations
            .subscribe(data => {
                if (data.length > 0) {
                    data.forEach(invite => this.onInvite(invite));
                }
            });

        this.newInvitationSub = this
            .chatService
            .newInvitation
            .subscribe(invite => this.onInvite(invite));

        this.userService.get();
        this.userService.getSubscriptions();
        this.directMessagingService.getUsers();
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof LayoutComponent
     */
    public ngOnDestroy() {
        this.onConnectedSub?.unsubscribe();
        this.onReconnectedSub?.unsubscribe();
        this.invitationsSub?.unsubscribe();
        this.newInvitationSub?.unsubscribe();
        this.userUpdatedSub?.unsubscribe();
    }

    /**
     * Handles an incoming invitation to the user.
     * @private
     * @param {Invitation} invitation The invitation instance.
     * @memberof LayoutComponent
     */
    private onInvite(invitation: Invitation) {
        const title = 'New invite';
        const content = `A user has invited you to join the server: ${invitation.server.name}`;
        this.notifyService.notify(title, content);
    }

}
