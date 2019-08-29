import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddServerComponent } from 'src/app/modules/servers/components/add-server/add-server.component';
import { EditServerComponent } from 'src/app/modules/servers/components/edit-server/edit-server.component';

const routes: Routes = [
    {
        path: 'add',
        component: AddServerComponent,
        pathMatch: 'full'
    },
    {
        path: 'edit/:id',
        component: EditServerComponent,
        pathMatch: 'full'
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ServersRoutingModule { }

