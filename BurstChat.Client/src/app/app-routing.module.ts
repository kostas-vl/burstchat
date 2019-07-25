import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { environment } from 'src/environments/environment';

const routes: Routes = [
    {
        path: 'core',
        loadChildren: () => import('src/app/modules/burst/burst.module').then(m => m.BurstModule)
    },
    {
        path: 'session',
        loadChildren: () => import('src/app/modules/session/session.module').then(m => m.SessionModule)
    },
    {
        path: '',
        redirectTo: '/session/login',
        pathMatch: 'full'
    },
    {
      path: '**',
      redirectTo: '/session/login'
    }
];

@NgModule({
    imports: [
        RouterModule.forRoot(routes, { useHash: !environment.production })
    ],
    exports: [
        RouterModule
    ]
})
export class AppRoutingModule { }
