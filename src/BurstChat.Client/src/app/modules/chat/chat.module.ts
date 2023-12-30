import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VirtualScrollerModule } from '@iharbeck/ngx-virtual-scroller';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { ChatRoutingModule } from 'src/app/modules/chat/chat.routing';
import { ChatRootComponent } from 'src/app/modules/chat/components/chat-root/chat-root.component';
import { ChatChannelComponent } from 'src/app/modules/chat/components/chat-channel/chat-channel.component';
import { ChatDirectComponent } from 'src/app/modules/chat/components/chat-direct/chat-direct.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';
import { ExpanderComponent } from 'src/app/components/shared/expander/expander.component';
import { MessageEditDialogComponent } from 'src/app/components/chat/message-edit-dialog/message-edit-dialog.component';
import { MessageDeleteDialogComponent } from 'src/app/components/chat/message-delete-dialog/message-delete-dialog.component';
import { ChatMessageComponent } from 'src/app/components/chat/chat-message/chat-message.component';
import { ChatMessagesComponent } from 'src/app/components/chat/chat-messages/chat-messages.component';
import { ChatInfoComponent } from 'src/app/components/chat/chat-info/chat-info.component';
import { ChatCallComponent } from 'src/app/components/chat/chat-call/chat-call.component';
import { ChatInputComponent } from 'src/app/components/chat/chat-input/chat-input.component';

import {
    faPaperPlane,
    faCommentAlt,
    faLock,
    faComments,
    faPhone,
    faPhoneSlash,
    faUserCircle,
    faMicrophone,
    faMicrophoneSlash,
    faVolumeUp,
    faVolumeMute,
    faPen,
    faTrashAlt,
    faQuestionCircle,
    faVideo,
    faClone,
} from '@fortawesome/free-solid-svg-icons';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        VirtualScrollerModule,
        FontAwesomeModule,
        ChatRoutingModule,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        DialogComponent,
        AvatarComponent,
        ExpanderComponent,
        MessageEditDialogComponent,
        MessageDeleteDialogComponent,
        ChatMessageComponent,
        ChatMessagesComponent,
        ChatInfoComponent,
        ChatCallComponent,
        ChatInputComponent
    ],
    declarations: [
        ChatRootComponent,
        ChatChannelComponent,
        ChatDirectComponent,
    ]
})
export class ChatModule {

    /**
     * Creates an instance of ChatModule.
     * @memberof ChatModule
     */
    constructor() {
        library.add(
            faPaperPlane,
            faCommentAlt,
            faLock,
            faComments,
            faPhone,
            faPhoneSlash,
            faUserCircle,
            faMicrophone,
            faMicrophoneSlash,
            faVolumeUp,
            faVolumeMute,
            faPen,
            faTrashAlt,
            faQuestionCircle,
            faVideo,
            faClone);
    }

}
