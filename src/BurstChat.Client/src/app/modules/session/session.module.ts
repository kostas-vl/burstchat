import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { SessionRoutingModule } from 'src/app/modules/session/session.routing';
import { SessionService } from 'src/app/services/session/session.service';
import { LogoutComponent } from 'src/app/modules/session/components/logout/logout.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';

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
        LogoutComponent,
    ],
    providers: [
        SessionService,
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
