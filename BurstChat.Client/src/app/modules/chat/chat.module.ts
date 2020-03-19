import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faPaperPlane, faCommentAlt, faLock, faComments, faPhone } from '@fortawesome/free-solid-svg-icons';
import { SharedModule } from 'src/app/modules/shared/shared.module';
import { ChatRoutingModule } from 'src/app/modules/chat/chat.routing';
import { ChatRootComponent } from 'src/app/modules/chat/components/chat-root/chat-root.component';
import { ChatChannelComponent } from 'src/app/modules/chat/components/chat-channel/chat-channel.component';
import { ChatGroupComponent } from 'src/app/modules/chat/components/chat-group/chat-group.component';
import { ChatDirectComponent } from 'src/app/modules/chat/components/chat-direct/chat-direct.component';
import { ChatInfoComponent } from 'src/app/modules/chat/components/chat-info/chat-info.component';
import { ChatMessagesComponent } from 'src/app/modules/chat/components/chat-messages/chat-messages.component';
import { ChatInputComponent } from 'src/app/modules/chat/components/chat-input/chat-input.component';
import { ChatMessageComponent } from 'src/app/modules/chat/components/chat-message/chat-message.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ScrollingModule,
        FontAwesomeModule,
        SharedModule,
        ChatRoutingModule,
    ],
    declarations: [
        ChatRootComponent,
        ChatChannelComponent,
        ChatGroupComponent,
        ChatDirectComponent,
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
        library.add(faPaperPlane,
                    faCommentAlt,
                    faLock,
                    faComments,
                    faPhone);
    }

}
