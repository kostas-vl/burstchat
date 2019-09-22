import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faCommentAlt } from '@fortawesome/free-solid-svg-icons';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { ChatRoutingModule } from 'src/app/modules/chat/chat-routing.module';
import { ChatRootComponent } from 'src/app/modules/chat/components/chat-root/chat-root.component';
import { ChatInfoComponent } from 'src/app/modules/chat/components/chat-info/chat-info.component';
import { ChatMessagesComponent } from 'src/app/modules/chat/components/chat-messages/chat-messages.component';
import { ChatInputComponent } from 'src/app/modules/chat/components/chat-input/chat-input.component';
import { ChatMessageComponent } from 'src/app/modules/chat/components/chat-message/chat-message.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        FontAwesomeModule,
        SharedModule,
        ChatRoutingModule,
    ],
    declarations: [
        ChatRootComponent,
        ChatInfoComponent,
        ChatMessagesComponent,
        ChatMessageComponent,
        ChatInputComponent,
    ],
})
export class ChatModule {

    /**
     * Creates an instance of ChatModule.
     * @memberof ChatModule
     */
    constructor() {
        library.add(faCommentAlt);
    }

}
