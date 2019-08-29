import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { ServersRoutingModule } from 'src/app/modules/servers/servers.routing';
import { AuthHttpInterceptor } from 'src/app/services/auth-http-interceptor/auth-http-interceptor.service';
import { ServersService } from 'src/app/modules/servers/services/servers/servers.service';
import { AddServerComponent } from 'src/app/modules/servers/components/add-server/add-server.component';
import { EditServerComponent } from 'src/app/modules/servers/components/edit-server/edit-server.component';

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
        EditServerComponent
    ],
    providers: [
        ServersService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthHttpInterceptor,
            multi: true
        }
    ]
})
export class ServersModule { }
