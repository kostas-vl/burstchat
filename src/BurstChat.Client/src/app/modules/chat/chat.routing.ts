import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatChannelComponent } from 'src/app/components/chat/chat-channel/chat-channel.component';
import { ChatDirectComponent } from 'src/app/components/chat/chat-direct/chat-direct.component';

const routes: Routes = [
    {
        path: 'channel',
        component: ChatChannelComponent
    },
    {
        path: 'direct',
        component: ChatDirectComponent
    },
    {
        path: '',
        redirectTo: '/core/home',
        pathMatch: 'full'
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
