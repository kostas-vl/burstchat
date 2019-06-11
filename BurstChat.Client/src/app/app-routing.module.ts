import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { environment } from 'src/environments/environment';

const routes: Routes = [
    {
        path: 'chat',
        loadChildren: () => import('src/app/modules/chat/chat.module').then(m => m.ChatModule)
    },
    {
        path: '',
        redirectTo: '/chat',
        pathMatch: 'full'
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
