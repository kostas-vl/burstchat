import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { SessionRoutingModule } from 'src/app/modules/session/session.routing';
import { SessionService } from 'src/app/modules/session/services/session-service/session.service'
import { LoginComponent } from 'src/app/modules/session/components/login/login.component';
import { LogoutComponent } from 'src/app/modules/session/components/logout/logout.component';
import { ResetPasswordComponent } from 'src/app/modules/session/components/reset-password/reset-password.component';
import { ChangePasswordComponent } from 'src/app/modules/session/components/change-password/change-password.component';
import { RegisterComponent } from 'src/app/modules/session/components/register/register.component';

@NgModule({
    imports: [
        CommonModule,
        HttpClientModule,
        FormsModule,
        FontAwesomeModule,
        SharedModule,
        SessionRoutingModule
    ],
    declarations: [
        LoginComponent,
        LogoutComponent,
        ResetPasswordComponent,
        ChangePasswordComponent,
        RegisterComponent
    ],
    providers: [
        SessionService
    ]
})
export class SessionModule {

    /**
     * Creats an instance of SessionModule
     * @memberof SessionModule
     */
    constructor() {
        library.add(faDragon);
    }

}
