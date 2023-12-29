import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LogoutComponent } from 'src/app/modules/session/components/logout/logout.component';
import { ResetPasswordComponent } from 'src/app/modules/session/components/reset-password/reset-password.component';
import { ChangePasswordComponent } from 'src/app/modules/session/components/change-password/change-password.component';

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
    {
        path: 'change',
        component: ChangePasswordComponent,
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
