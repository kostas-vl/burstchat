import { Routes } from '@angular/router';
import { ChatChannelComponent } from 'src/app/components/chat/chat-channel/chat-channel.component';
import { ChatDirectComponent } from 'src/app/components/chat/chat-direct/chat-direct.component';

export const routes: Routes = [
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
