import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faUserCircle } from '@fortawesome/free-solid-svg-icons';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { UserRoutingModule } from 'src/app/modules/user/user.routing';
import { UrlInterceptorService } from 'src/app/services/url-interceptor/url-interceptor.service';
import { EditUserComponent } from 'src/app/modules/user/components/edit-user/edit-user.component';
import { EditUserInfoComponent } from 'src/app/modules/user/components/edit-user-info/edit-user-info.component';
import { EditUserInvitationsComponent } from 'src/app/modules/user/components/edit-user-invitations/edit-user-invitations.component';
import { EditUserMediaComponent } from 'src/app/modules/user/components/edit-user-media/edit-user-media.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        FontAwesomeModule,
        SharedModule,
        UserRoutingModule
    ],
    declarations: [
        EditUserComponent,
        EditUserInfoComponent,
        EditUserInvitationsComponent,
        EditUserMediaComponent
    ],
    providers: []
})
export class UserModule {

    /**
     * Creates an instance of UserModule.
     * @memberof UserModule
     */
    constructor() {
        library.add(faUserCircle);
    }

}
