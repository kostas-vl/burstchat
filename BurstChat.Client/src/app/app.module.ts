import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { StorageService } from 'src/app/services/storage/storage.service';
import { AuthenticationGuardService } from 'src/app/services/authentication-guard/authentication-guard.service';
import { RootComponent } from 'src/app/components/root/root.component';
import { NotificationComponent } from 'src/app/components/notification/notification.component';
import { NotificationListComponent } from 'src/app/components/notification-list/notification-list.component';

@NgModule({
    imports: [
        BrowserModule,
        FontAwesomeModule,
        AppRoutingModule
    ],
    declarations: [
        RootComponent,
        NotificationComponent,
        NotificationListComponent,
    ],
    providers: [
        NotifyService,
        StorageService,
        AuthenticationGuardService
    ],
    bootstrap: [RootComponent]
})
export class AppModule {

    /**
     * Creates an instance of AppModule.
     * @memberof AppModule
     */
    constructor() {
        library.add(faTimes);
    }

}
