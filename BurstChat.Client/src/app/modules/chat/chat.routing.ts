import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatRootComponent } from 'src/app/modules/chat/components/chat-root/chat-root.component';

const routes: Routes = [
    {
        path: '',
        component: ChatRootComponent,
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
