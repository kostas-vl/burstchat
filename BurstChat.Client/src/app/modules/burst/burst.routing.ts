import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from 'src/app/modules/burst/components/layout/layout.component';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            {
                path: 'chat/private/:id',
                loadChildren: () => import('src/app/modules/chat/chat.module').then(m => m.ChatModule)
            },
            {
                path: 'chat/channel/:id',
                loadChildren: () => import('src/app/modules/chat/chat.module').then(m => m.ChatModule)
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

