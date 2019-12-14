import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { BurstRoutingModule } from 'src/app/modules/burst/burst.routing';
import { AuthHttpInterceptor } from 'src/app/services/auth-http-interceptor/auth-http-interceptor.service';
import { UrlInterceptorService } from 'src/app/services/url-interceptor/url-interceptor.service';
import { SidebarComponent } from 'src/app/modules/burst/components/sidebar/sidebar.component';
import { SidebarUserInfoComponent } from 'src/app/modules/burst/components/sidebar-user-info/sidebar-user-info.component';
import { ServerListComponent } from 'src/app/modules/burst/components/server-list/server-list.component';
import { ServerComponent } from 'src/app/modules/burst/components/server/server.component';
import { ServerInfoComponent } from 'src/app/modules/burst/components/server-info/server-info.component';
import { ChannelListComponent } from 'src/app/modules/burst/components/channel-list/channel-list.component';
import { ChannelComponent } from 'src/app/modules/burst/components/channel/channel.component';
import { LayoutComponent } from 'src/app/modules/burst/components/layout/layout.component';
import { UserListComponent } from 'src/app/modules/burst/components/user-list/user-list.component';
import { UserComponent } from 'src/app/modules/burst/components/user/user.component';

import { library } from '@fortawesome/fontawesome-svg-core';
import {
    faDragon,
    faChalkboardTeacher,
    faPlus,
    faDatabase,
    faCommentAlt,
    faSignOutAlt,
    faCircle,
    faDotCircle
} from '@fortawesome/free-solid-svg-icons';

@NgModule({
    imports: [
        CommonModule,
        HttpClientModule,
        FontAwesomeModule,
        BurstRoutingModule
    ],
    declarations: [
        LayoutComponent,
        SidebarComponent,
        SidebarUserInfoComponent,
        ServerListComponent,
        ServerComponent,
        ServerInfoComponent,
        ChannelListComponent,
        ChannelComponent,
        UserListComponent,
        UserComponent
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: UrlInterceptorService,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthHttpInterceptor,
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
        library.add(faDragon,
            faChalkboardTeacher,
            faPlus,
            faDatabase,
            faCommentAlt,
            faSignOutAlt,
            faCircle,
            faDotCircle);
    }

}
