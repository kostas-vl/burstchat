import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { environment } from 'src/environments/environment';
import { AuthenticationGuardService } from 'src/app/services/authentication-guard/authentication-guard.service';

const routes: Routes = [
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

@NgModule({
    imports: [
        RouterModule.forRoot(routes, { useHash: !environment.production, relativeLinkResolution: 'legacy' })
    ],
    exports: [
        RouterModule
    ]
})
export class AppRoutingModule { }
