import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ServersRoutingModule } from 'src/app/modules/servers/servers.routing';
import { EditServerComponent } from 'src/app/modules/servers/components/edit-server/edit-server.component';
import { EditServerInfoComponent } from 'src/app/modules/servers/components/edit-server-info/edit-server-info.component';
import { EditServerChannelsComponent } from 'src/app/modules/servers/components/edit-server-channels/edit-server-channels.component';
import { EditServerUsersComponent } from 'src/app/modules/servers/components/edit-server-users/edit-server-users.component';
import { CardComponent } from 'src/app/components/card/card.component';
import { CardHeaderComponent } from 'src/app/components/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/card-footer/card-footer.component';
import { ImageCropComponent } from 'src/app/components/image-crop/image-crop.component';
import { DialogComponent } from 'src/app/components/dialog/dialog.component';
import { AvatarComponent } from 'src/app/components/avatar/avatar.component';

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
        AvatarComponent
    ],
    declarations: [
        EditServerComponent,
        EditServerInfoComponent,
        EditServerChannelsComponent,
        EditServerUsersComponent
    ],
    providers: []
})
export class ServersModule { }
