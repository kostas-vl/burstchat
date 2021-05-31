import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faTimes, faBomb, faInfoCircle, faExclamationCircle, faCheck } from '@fortawesome/free-solid-svg-icons';
import { AppRoutingModule } from 'src/app/app.routing';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { StorageService } from 'src/app/services/storage/storage.service';
import { AuthenticationGuardService } from 'src/app/services/authentication-guard/authentication-guard.service';
import { RootComponent } from 'src/app/components/root/root.component';
import { PopupListComponent } from 'src/app/components/popup-list/popup-list.component';
import { PopupComponent } from 'src/app/components/popup/popup.component';

@NgModule({
    imports: [
        BrowserModule,
        FontAwesomeModule,
        AppRoutingModule
    ],
    declarations: [
        RootComponent,
        PopupListComponent,
        PopupComponent,
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
        library.add(faTimes, faBomb, faInfoCircle, faExclamationCircle, faCheck);
    }

}
