import { Component, effect } from '@angular/core';
import { User } from 'src/app/models/user/user';
import { DirectMessagingService } from 'src/app/services/direct-messaging/direct-messaging.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { ExpanderComponent } from 'src/app/components/shared/expander/expander.component';
import { UserComponent } from 'src/app/components/core/user/user.component';

/**
 * This class represents an angular component that displays the list of all direct messaging chats of the user.
 * @export
 * @class DirectMessagingListComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-direct-messaging-list',
    templateUrl: './direct-messaging-list.component.html',
    styleUrl: './direct-messaging-list.component.scss',
    standalone: true,
    imports: [ExpanderComponent, UserComponent]
})
export class DirectMessagingListComponent {

    public users: User[] = [];

    /**
     * Creates an instance of DirectMessagingListComponent.
     * @memberof DirectMessagingListComponent
     */
    constructor(
        private directMessagingService: DirectMessagingService,
        private chatService: ChatService
    ) {
        effect(() => this.users = this.directMessagingService.users());
        effect(() => {
            const user = this.chatService.userUpdated();

            if (!user) return;

            const entry = this.users.find(u => u.id === user.id);
            if (entry) {
                const index = this.users.indexOf(entry);
                this.users[index] = { ...user };
            }
        });
    }

}
