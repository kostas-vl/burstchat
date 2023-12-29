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
import { ServerComponent } from 'src/app/modules/burst/components/server/server.component';
import { ChannelListComponent } from 'src/app/modules/burst/components/channel-list/channel-list.component';
import { ChannelComponent } from 'src/app/modules/burst/components/channel/channel.component';
import { LayoutComponent } from 'src/app/modules/burst/components/layout/layout.component';
import { UserListComponent } from 'src/app/modules/burst/components/user-list/user-list.component';
import { IncomingCallComponent } from 'src/app/modules/burst/components/incoming-call/incoming-call.component';
import { OngoingCallComponent } from 'src/app/modules/burst/components/ongoing-call/ongoing-call.component';
import { AddServerComponent } from 'src/app/modules/burst/components/add-server/add-server.component';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';
import { ExpanderComponent } from 'src/app/components/shared/expander/expander.component';
import { ServerInfoComponent } from 'src/app/components/core/server-info/server-info.component';
import { UserComponent } from 'src/app/components/core/user/user.component';

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
    ],
    declarations: [
        LayoutComponent,
        SidebarComponent,
        SidebarUserInfoComponent,
        SidebarSelectionComponent,
        DirectMessagingComponent,
        DirectMessagingListComponent,
        ServerComponent,
        ChannelListComponent,
        ChannelComponent,
        UserListComponent,
        IncomingCallComponent,
        OngoingCallComponent,
        AddServerComponent
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
