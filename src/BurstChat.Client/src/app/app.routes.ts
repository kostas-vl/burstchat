import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { Routes } from '@angular/router';
import { authenticationGuard} from 'src/app/guards/authentication/authentication.guard';
import { authInterceptor } from 'src/app/interceptors/auth/auth.interceptor';
import { endpointInterceptor } from 'src/app/interceptors/endpoint/endpoint.interceptor';

export const routes: Routes = [
    {
        path: 'core',
        loadChildren: () => import('src/app/modules/burst/burst.module').then(m => m.BurstModule),
        canActivate: [authenticationGuard],
        providers: [provideHttpClient(withInterceptors([endpointInterceptor, authInterceptor]))]
    },
    {
        path: 'session',
        loadChildren: () => import('src/app/components/session/session.routes').then(c => c.routes),
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
