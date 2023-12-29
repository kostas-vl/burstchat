import { Routes } from '@angular/router';
import { LoginComponent } from 'src/app/components/session/login/login.component';
import { RegisterComponent } from 'src/app/components/session/register/register.component';
import { ChangePasswordComponent } from 'src/app/components/session/change-password/change-password.component';
import { ResetPasswordComponent } from 'src/app/components/session/reset-password/reset-password.component';

export const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent,
        pathMatch: 'full'
    },
    {
        path: 'register',
        component: RegisterComponent,
        pathMatch: 'full'
    },
    {
        path: 'change',
        component: ChangePasswordComponent,
        pathMatch: 'full'
    },
    {
        path: 'reset',
        component: ResetPasswordComponent,
        pathMatch: 'full'
    },
];
