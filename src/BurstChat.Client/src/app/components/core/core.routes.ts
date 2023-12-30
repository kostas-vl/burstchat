import { Routes } from '@angular/router';
import { LayoutComponent } from 'src/app/components/core/layout/layout.component';
import { HomeComponent } from 'src/app/components/core/home/home.component';

export const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            {
                path: 'home',
                component: HomeComponent,
                pathMatch: 'full'
            },
            {
                path: 'servers',
                loadChildren: () => import('src/app/components/server/server.routes').then(c => c.routes)
            },
            {
                path: 'chat',
                loadChildren: () => import('src/app/components/chat/chat.routes').then(c => c.routes)
            },
            {
                path: 'user',
                loadChildren: () => import('src/app/components/user/user.routes').then(c => c.routes)
            }
        ]
    }
];
