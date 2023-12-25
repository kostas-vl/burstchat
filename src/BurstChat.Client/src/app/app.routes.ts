import { Routes } from '@angular/router';
import { AuthenticationGuardService } from 'src/app/services/authentication-guard/authentication-guard.service';

export const routes: Routes = [
    {
        path: 'core',
        loadChildren: () => import('src/app/modules/burst/burst.module').then(m => m.BurstModule),
        canActivate: [AuthenticationGuardService]
    },
    {
        path: 'session',
        loadChildren: () => import('src/app/modules/session/session.module').then(m => m.SessionModule)
    },
    {
        path: '',
        redirectTo: '/core/home',
        pathMatch: 'full'
    },
    {
      path: '**',
      redirectTo: '/session/login'
    }
];
