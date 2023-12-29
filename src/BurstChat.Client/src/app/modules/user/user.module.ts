import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faUserCircle } from '@fortawesome/free-solid-svg-icons';
import { UserRoutingModule } from 'src/app/modules/user/user.routing';
import { EditUserComponent } from 'src/app/modules/user/components/edit-user/edit-user.component';
import { EditUserInfoComponent } from 'src/app/modules/user/components/edit-user-info/edit-user-info.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { ImageCropComponent } from 'src/app/components/shared/image-crop/image-crop.component';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';
import { ExpanderComponent } from 'src/app/components/shared/expander/expander.component';
import { EditUserMediaComponent } from 'src/app/components/user/edit-user-media/edit-user-media.component';
import { EditUserInvitationsComponent } from 'src/app/components/user/edit-user-invitations/edit-user-invitations.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        FontAwesomeModule,
        UserRoutingModule,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        ImageCropComponent,
        DialogComponent,
        AvatarComponent,
        ExpanderComponent,
        EditUserMediaComponent,
        EditUserInvitationsComponent
    ],
    declarations: [
        EditUserComponent,
        EditUserInfoComponent,
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
