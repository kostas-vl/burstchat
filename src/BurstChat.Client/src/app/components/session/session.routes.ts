import { Routes } from '@angular/router';
import { LoginComponent } from 'src/app/components/session/login/login.component';
import { RegisterComponent } from 'src/app/components/session/register/register.component';

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
    }
];
