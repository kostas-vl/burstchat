import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { SessionRoutingModule } from 'src/app/modules/session/session.routing';
import { UrlInterceptorService } from 'src/app/services/url-interceptor/url-interceptor.service';
import { SessionService } from 'src/app/modules/session/services/session-service/session.service';
import { LoginComponent } from 'src/app/modules/session/components/login/login.component';
import { LogoutComponent } from 'src/app/modules/session/components/logout/logout.component';
import { ResetPasswordComponent } from 'src/app/modules/session/components/reset-password/reset-password.component';
import { ChangePasswordComponent } from 'src/app/modules/session/components/change-password/change-password.component';
import { RegisterComponent } from 'src/app/modules/session/components/register/register.component';
import { CardComponent } from 'src/app/components/card/card.component';
import { CardHeaderComponent } from 'src/app/components/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/card-footer/card-footer.component';

@NgModule({
    imports: [
        CommonModule,
        HttpClientModule,
        FormsModule,
        FontAwesomeModule,
        SessionRoutingModule,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
    ],
    declarations: [
        LoginComponent,
        LogoutComponent,
        ResetPasswordComponent,
        ChangePasswordComponent,
        RegisterComponent
    ],
    providers: [
        SessionService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: UrlInterceptorService,
            multi: true
        }
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
