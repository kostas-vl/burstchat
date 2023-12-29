import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LogoutComponent } from 'src/app/modules/session/components/logout/logout.component';
import { ResetPasswordComponent } from 'src/app/modules/session/components/reset-password/reset-password.component';

const routes: Routes = [
    {
        path: 'logout',
        component: LogoutComponent,
        pathMatch: 'full'
    },
    {
        path: 'reset',
        component: ResetPasswordComponent,
        pathMatch: 'full'
    },
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SessionRoutingModule { }
