import { Routes } from '@angular/router';
import { authenticationGuard} from 'src/app/guards/authentication/authentication.guard';

export const routes: Routes = [
    {
        path: 'core',
        loadChildren: () => import('src/app/modules/burst/burst.module').then(m => m.BurstModule),
        canActivate: [authenticationGuard]
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
