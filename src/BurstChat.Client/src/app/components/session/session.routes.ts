import { Routes } from '@angular/router';
import { LoginComponent } from 'src/app/components/session/login/login.component';

export const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent,
        pathMatch: 'full'
    }
];
