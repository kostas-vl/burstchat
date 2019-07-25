import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faSmile, faDragon } from '@fortawesome/free-solid-svg-icons';
import { BurstRoutingModule } from 'src/app/modules/burst/burst.routing';
import { SidebarComponent } from 'src/app/modules/burst/components/sidebar/sidebar.component';
import { ServerListComponent } from 'src/app/modules/burst/components/server-list/server-list.component';
import { ServerComponent } from 'src/app/modules/burst/components/server/server.component';
import { UserListComponent } from 'src/app/modules/burst/components/user-list/user-list.component';
import { UserComponent } from 'src/app/modules/burst/components/user/user.component';
import { LayoutComponent } from 'src/app/modules/burst/components/layout/layout.component';

@NgModule({
    imports: [
        CommonModule,
        FontAwesomeModule,
        BurstRoutingModule
    ],
    declarations: [
        LayoutComponent,
        SidebarComponent,
        ServerListComponent,
        ServerComponent,
        UserListComponent,
        UserComponent
    ],
})
export class BurstModule {

    /**
     * Creates an instance of BurstModule.
     * @memberof BurstModule
     */
    constructor() {
        library.add(faSmile, faDragon);
    }

}
