import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ServersRoutingModule } from 'src/app/modules/servers/servers.routing';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { ImageCropComponent } from 'src/app/components/shared/image-crop/image-crop.component';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';
import { EditServerUsersComponent } from 'src/app/components/server/edit-server-users/edit-server-users.component';
import { EditServerInfoComponent } from 'src/app/components/server/edit-server-info/edit-server-info.component';
import { EditServerChannelsComponent } from 'src/app/components/server/edit-server-channels/edit-server-channels.component';
import { EditServerComponent } from 'src/app/components/server/edit-server/edit-server.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        ServersRoutingModule,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        ImageCropComponent,
        DialogComponent,
        AvatarComponent,
        EditServerUsersComponent,
        EditServerInfoComponent,
        EditServerChannelsComponent,
        EditServerComponent
    ]
})
export class ServersModule { }
