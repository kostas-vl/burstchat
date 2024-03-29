import { Component, OnInit, effect } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Invitation } from 'src/app/models/servers/invitation';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SidebarService } from 'src/app/services/sidebar/sidebar.service';
import { UserService } from 'src/app/services/user/user.service';
import { ServersService } from 'src/app/services/servers/servers.service';
import { ChannelsService } from 'src/app/services/channels/channels.service';
import { DirectMessagingService } from 'src/app/services/direct-messaging/direct-messaging.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { MediaService } from 'src/app/services/media/media.service';
import { RtcSessionService } from 'src/app/services/rtc-session/rtc-session.service';
import { SidebarComponent } from 'src/app/components/core/sidebar/sidebar.component';
import { IncomingCallComponent } from 'src/app/components/core/incoming-call/incoming-call.component';
import { AddServerComponent } from 'src/app/components/core/add-server/add-server.component';

/**
 * This class represents an angular component that displays on screen all components of
 * the burst chat client.
 * @class LayoutComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-layout',
    templateUrl: './layout.component.html',
    styleUrl: './layout.component.scss',
    standalone: true,
    imports: [
        RouterOutlet,
        SidebarComponent,
        IncomingCallComponent,
        AddServerComponent
    ],
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
export class LayoutComponent implements OnInit {

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
    ) {
        effect(() => {
            if (this.chatService.onConnected()) {
                setTimeout(() => {
                    this.chatService.getInvitations();
                    this.loading = false;
                }, 300);
            }
        });
        effect(() => {
            if (this.chatService.onReconnected()) {
                this.chatService.getInvitations();
            }
        });
        effect(() => {
            const data = this.chatService.invitations();
            if (data.length > 0) {
                data.forEach(invite => this.onInvite(invite));
            }
        });
        effect(() => this.onInvite(this.chatService.newInvitation()));
    }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof LayoutComponent
     */
    public ngOnInit() {
        this.userService.get();
        this.userService.getSubscriptions();
        this.directMessagingService.getUsers();
    }

    /**
     * Handles an incoming invitation to the user.
     * @private
     * @param {Invitation | null} invitation The invitation instance.
     * @memberof LayoutComponent
     */
    private onInvite(invitation: Invitation | null) {
        if (!invitation) return;

        const title = 'New invite';
        const content = `A user has invited you to join the server: ${invitation.server.name}`;
        this.notifyService.notify(title, content);
    }

}
