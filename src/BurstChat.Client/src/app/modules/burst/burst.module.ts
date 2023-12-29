import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { SharedModule } from 'src/app/modules/shared/shared.module';
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
        SharedModule,
        BurstRoutingModule
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
