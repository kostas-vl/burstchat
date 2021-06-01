import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EditServerComponent } from 'src/app/modules/servers/components/edit-server/edit-server.component';

const routes: Routes = [
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

