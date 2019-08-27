import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faSmile, faDragon, faUserCircle, faCog, faPlus } from '@fortawesome/free-solid-svg-icons';
import { BurstRoutingModule } from 'src/app/modules/burst/burst.routing';
import { AuthHttpInterceptor } from 'src/app/services/auth-http-interceptor/auth-http-interceptor.service';
import { UserService } from 'src/app/modules/burst/services/user/user.service';
import { SidebarComponent } from 'src/app/modules/burst/components/sidebar/sidebar.component';
import { SidebarUserInfoComponent } from 'src/app/modules/burst/components/sidebar-user-info/sidebar-user-info.component';
import { ServerListComponent } from 'src/app/modules/burst/components/server-list/server-list.component';
import { ServerComponent } from 'src/app/modules/burst/components/server/server.component';
import { UserListComponent } from 'src/app/modules/burst/components/user-list/user-list.component';
import { UserComponent } from 'src/app/modules/burst/components/user/user.component';
import { LayoutComponent } from 'src/app/modules/burst/components/layout/layout.component';

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
        UserListComponent,
        UserComponent
    ],
    providers: [
        UserService,
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
        library.add(faSmile,
                    faDragon,
                    faUserCircle,
                    faCog,
                    faPlus);
    }

}
