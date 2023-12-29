import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { BurstRoutingModule } from 'src/app/modules/burst/burst.routing';
import { SidebarComponent } from 'src/app/modules/burst/components/sidebar/sidebar.component';
import { SidebarUserInfoComponent } from 'src/app/modules/burst/components/sidebar-user-info/sidebar-user-info.component';
import { SidebarSelectionComponent } from 'src/app/modules/burst/components/sidebar-selection/sidebar-selection.component';
import { DirectMessagingComponent } from 'src/app/modules/burst/components/direct-messaging/direct-messaging.component';
import { DirectMessagingListComponent } from './components/direct-messaging-list/direct-messaging-list.component';
import { LayoutComponent } from 'src/app/modules/burst/components/layout/layout.component';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';
import { ExpanderComponent } from 'src/app/components/shared/expander/expander.component';
import { ServerInfoComponent } from 'src/app/components/core/server-info/server-info.component';
import { UserComponent } from 'src/app/components/core/user/user.component';
import { UserListComponent } from 'src/app/components/core/user-list/user-list.component';
import { ServerComponent } from 'src/app/components/core/server/server.component';
import { IncomingCallComponent } from 'src/app/components/core/incoming-call/incoming-call.component';
import { AddServerComponent } from 'src/app/components/core/add-server/add-server.component';
import { OngoingCallComponent } from 'src/app/components/core/ongoing-call/ongoing-call.component';
import { ChannelComponent } from 'src/app/components/core/channel/channel.component';

import { library } from '@fortawesome/fontawesome-svg-core';
import {
    faDragon,
    faCog,
    faDatabase,
    faCommentAlt,
    faSignOutAlt,
    faCircle,
    faDotCircle,
    faCubes,
    faComments,
    faCheck,
    faTimes,
    faUserCircle,
    faMicrophone,
    faMicrophoneSlash,
    faVolumeUp,
    faVolumeMute,
    faExternalLinkAlt
} from '@fortawesome/free-solid-svg-icons';
import { ChannelListComponent } from 'src/app/components/core/channel-list/channel-list.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        FontAwesomeModule,
        BurstRoutingModule,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        DialogComponent,
        AvatarComponent,
        ExpanderComponent,
        ServerInfoComponent,
        UserComponent,
        UserListComponent,
        ServerComponent,
        IncomingCallComponent,
        AddServerComponent,
        OngoingCallComponent,
        ChannelComponent,
        ChannelListComponent
    ],
    declarations: [
        LayoutComponent,
        SidebarComponent,
        SidebarUserInfoComponent,
        SidebarSelectionComponent,
        DirectMessagingComponent,
        DirectMessagingListComponent,
    ]
})
export class BurstModule {

    /**
     * Creates an instance of BurstModule.
     * @memberof BurstModule
     */
    constructor() {
        library.add(
            faDragon,
            faCog,
            faCubes,
            faDatabase,
            faCommentAlt,
            faSignOutAlt,
            faCircle,
            faDotCircle,
            faComments,
            faCheck,
            faTimes,
            faUserCircle,
            faMicrophone,
            faMicrophoneSlash,
            faVolumeUp,
            faVolumeMute,
            faExternalLinkAlt
        );
    }

}
