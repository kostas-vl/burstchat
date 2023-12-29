import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { BurstRoutingModule } from 'src/app/modules/burst/burst.routing';
import { AuthHttpInterceptor } from 'src/app/modules/burst/services/auth-http-interceptor/auth-http-interceptor.service';
import { UrlInterceptorService } from 'src/app/services/url-interceptor/url-interceptor.service';
import { SidebarComponent } from 'src/app/modules/burst/components/sidebar/sidebar.component';
import { SidebarUserInfoComponent } from 'src/app/modules/burst/components/sidebar-user-info/sidebar-user-info.component';
import { SidebarSelectionComponent } from 'src/app/modules/burst/components/sidebar-selection/sidebar-selection.component';
import { DirectMessagingComponent } from 'src/app/modules/burst/components/direct-messaging/direct-messaging.component';
import { DirectMessagingListComponent } from './components/direct-messaging-list/direct-messaging-list.component';
import { ServerComponent } from 'src/app/modules/burst/components/server/server.component';
import { ServerInfoComponent } from 'src/app/modules/burst/components/server-info/server-info.component';
import { ChannelListComponent } from 'src/app/modules/burst/components/channel-list/channel-list.component';
import { ChannelComponent } from 'src/app/modules/burst/components/channel/channel.component';
import { LayoutComponent } from 'src/app/modules/burst/components/layout/layout.component';
import { UserListComponent } from 'src/app/modules/burst/components/user-list/user-list.component';
import { UserComponent } from 'src/app/modules/burst/components/user/user.component';
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
        HttpClientModule,
        FormsModule,
        FontAwesomeModule,
        BurstRoutingModule,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        DialogComponent,
        AvatarComponent,
        ExpanderComponent
    ],
    declarations: [
        LayoutComponent,
        SidebarComponent,
        SidebarUserInfoComponent,
        SidebarSelectionComponent,
        DirectMessagingComponent,
        DirectMessagingListComponent,
        ServerComponent,
        ServerInfoComponent,
        ChannelListComponent,
        ChannelComponent,
        UserListComponent,
        UserComponent,
        IncomingCallComponent,
        OngoingCallComponent,
        AddServerComponent
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthHttpInterceptor,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: UrlInterceptorService,
            multi: true
        }
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
