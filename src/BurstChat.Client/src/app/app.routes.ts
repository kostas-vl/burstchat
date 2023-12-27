import { Routes } from '@angular/router';
import { authenticationGuardFn } from 'src/app/services/authentication-guard/authentication-guard.service';

export const routes: Routes = [
    {
        path: 'core',
        loadChildren: () => import('src/app/modules/burst/burst.module').then(m => m.BurstModule),
        canActivate: [authenticationGuardFn]
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
