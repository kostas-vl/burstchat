import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatRootComponent } from 'src/app/modules/chat/components/chat-root/chat-root.component';
import { ChatChannelComponent } from 'src/app/modules/chat/components/chat-channel/chat-channel.component';
import { ChatGroupComponent } from 'src/app/modules/chat/components/chat-group/chat-group.component';
import { ChatDirectComponent } from 'src/app/modules/chat/components/chat-direct/chat-direct.component';
import { ChatCallComponent } from 'src/app/modules/chat/components/chat-call/chat-call.component';

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
        component: ChatGroupComponent
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
