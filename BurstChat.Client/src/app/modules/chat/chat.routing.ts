import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatRootComponent } from 'src/app/modules/chat/components/chat-root/chat-root.component';
import { ChatChannelComponent } from 'src/app/modules/chat/components/chat-channel/chat-channel.component';

const routes: Routes = [
    {
        path: '',
        component: ChatRootComponent,
    },
    {
        path: 'channel',
        component: ChatChannelComponent
    },
    {
        path: 'private',
        loadChildren: () => import('src/app/modules/chat/chat.module').then(m => m.ChatModule)
    },
];

@NgModule({
    imports: [
        RouterModule.forChild(routes),
    ],
    exports: [
        RouterModule
    ]
})
export class ChatRoutingModule { }
