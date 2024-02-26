import { Component, computed } from '@angular/core';
import { SidebarService } from 'src/app/services/sidebar/sidebar.service';
import { DisplayServer } from 'src/app/models/sidebar/display-server';
import { SidebarSelectionComponent } from 'src/app/components/core/sidebar-selection/sidebar-selection.component';
import { DirectMessagingListComponent } from 'src/app/components/core/direct-messaging-list/direct-messaging-list.component';
import { ServerInfoComponent } from 'src/app/components/core/server-info/server-info.component';
import { OngoingCallComponent } from 'src/app/components/core/ongoing-call/ongoing-call.component';
import { ChannelListComponent } from 'src/app/components/core/channel-list/channel-list.component';
import { UserListComponent } from 'src/app/components/core/user-list/user-list.component';
import { SidebarUserInfoComponent } from 'src/app/components/core/sidebar-user-info/sidebar-user-info.component';

/**
 * This class represents an angular component that displays on screen the main sidebar of the application.
 * @export
 * @class SidebarComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrl: './sidebar.component.scss',
    standalone: true,
    imports: [
        SidebarSelectionComponent,
        DirectMessagingListComponent,
        ServerInfoComponent,
        OngoingCallComponent,
        ChannelListComponent,
        UserListComponent,
        SidebarUserInfoComponent
    ]
})
export class SidebarComponent {

    public display = computed(() => {
        const options = this.sidebarService.display();
        if (options instanceof DisplayServer) {
            return 'server';
        } else {
            return 'direct';
        }
    });

    /**
     * Creates an instance of SidebarComponent.
     * @memberof SidebarComponent
     */
    constructor(private sidebarService: SidebarService) { }

}
