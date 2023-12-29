import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VirtualScrollerModule } from '@iharbeck/ngx-virtual-scroller';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { ChatRoutingModule } from 'src/app/modules/chat/chat.routing';
import { ChatRootComponent } from 'src/app/modules/chat/components/chat-root/chat-root.component';
import { ChatChannelComponent } from 'src/app/modules/chat/components/chat-channel/chat-channel.component';
import { ChatGroupComponent } from 'src/app/modules/chat/components/chat-group/chat-group.component';
import { ChatDirectComponent } from 'src/app/modules/chat/components/chat-direct/chat-direct.component';
import { ChatInfoComponent } from 'src/app/modules/chat/components/chat-info/chat-info.component';
import { ChatMessagesComponent } from 'src/app/modules/chat/components/chat-messages/chat-messages.component';
import { ChatInputComponent } from 'src/app/modules/chat/components/chat-input/chat-input.component';
import { ChatMessageComponent } from 'src/app/modules/chat/components/chat-message/chat-message.component';
import { ChatCallComponent } from 'src/app/modules/chat/components/chat-call/chat-call.component';
import { MessageEditDialogComponent } from 'src/app/modules/chat/components/message-edit-dialog/message-edit-dialog.component';
import { MessageDeleteDialogComponent } from 'src/app/modules/chat/components/message-delete-dialog/message-delete-dialog.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';
import { ExpanderComponent } from 'src/app/components/shared/expander/expander.component';

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
        ExpanderComponent
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
        ChatCallComponent,
        MessageEditDialogComponent,
        MessageDeleteDialogComponent
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
