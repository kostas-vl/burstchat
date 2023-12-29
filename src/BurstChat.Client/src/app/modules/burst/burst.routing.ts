import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from 'src/app/components/core/home/home.component';
import { LayoutComponent } from 'src/app/components/core/layout/layout.component';

const routes: Routes = [
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
                loadChildren: () => import('src/app/modules/servers/servers.module').then(m => m.ServersModule)
            },
            {
                path: 'chat',
                loadChildren: () => import('src/app/modules/chat/chat.module').then(m => m.ChatModule)
            },
            {
                path: 'user',
                loadChildren: () => import('src/app/modules/user/user.module').then(m => m.UserModule)
            }
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class BurstRoutingModule { }

