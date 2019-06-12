import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faSmile, faDragon } from '@fortawesome/free-solid-svg-icons';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { RootComponent } from 'src/app/components/root/root.component';
import { SidebarComponent } from 'src/app/components/sidebar/sidebar.component';
import { ServerListComponent } from 'src/app/components/server-list/server-list.component';
import { ServerComponent } from 'src/app/components/server/server.component';
import { UserListComponent } from 'src/app/components/user-list/user-list.component';
import { UserComponent } from 'src/app/components/user/user.component';

@NgModule({
    imports: [
        BrowserModule,
        FontAwesomeModule,
        AppRoutingModule
    ],
    declarations: [
        RootComponent,
        SidebarComponent,
        ServerListComponent,
        ServerComponent,
        UserListComponent,
        UserComponent
    ],
    providers: [],
    bootstrap: [RootComponent]
})
export class AppModule {

    /**
     * Creates an instance of AppModule.
     * @memberof AppModule
     */
    constructor() {
        library.add(faSmile, faDragon);
    }

}
