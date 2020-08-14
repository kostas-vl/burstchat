import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { ServersRoutingModule } from 'src/app/modules/servers/servers.routing';
import { UrlInterceptorService } from 'src/app/services/url-interceptor/url-interceptor.service';
import { AddServerComponent } from 'src/app/modules/servers/components/add-server/add-server.component';
import { EditServerComponent } from 'src/app/modules/servers/components/edit-server/edit-server.component';
import { EditServerInfoComponent } from 'src/app/modules/servers/components/edit-server-info/edit-server-info.component';
import { EditServerChannelsComponent } from 'src/app/modules/servers/components/edit-server-channels/edit-server-channels.component';
import { EditServerUsersComponent } from 'src/app/modules/servers/components/edit-server-users/edit-server-users.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        SharedModule,
        ServersRoutingModule
    ],
    declarations: [
        AddServerComponent,
        EditServerComponent,
        EditServerInfoComponent,
        EditServerChannelsComponent,
        EditServerUsersComponent
    ],
    providers: []
})
export class ServersModule { }
