import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatRootComponent } from 'src/app/modules/chat/components/chat-root/chat-root.component';
import { ChatDirectComponent } from 'src/app/modules/chat/components/chat-direct/chat-direct.component';
import { ChatChannelComponent } from 'src/app/components/chat/chat-channel/chat-channel.component';

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
        path: 'direct',
        component: ChatDirectComponent
    }
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
