import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { DirectMessagingService } from 'src/app/modules/burst/services/direct-messaging/direct-messaging.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

/**
 * This class represents an angular component that displays the list of all direct messaging chats of the user.
 * @export
 * @class DirectMessagingListComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-direct-messaging-list',
    templateUrl: './direct-messaging-list.component.html',
    styleUrls: ['./direct-messaging-list.component.scss']
})
export class DirectMessagingListComponent implements OnInit, OnDestroy {

    private subscriptions?: Subscription[];

    public users: User[] = [];

    /**
     * Creates an instance of DirectMessagingListComponent.
     * @memberof DirectMessagingListComponent
     */
    constructor(
        private directMessagingService: DirectMessagingService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof DirectMessagingListComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.directMessagingService
                .users
                .subscribe(users => this.users = users),
            this.chatService
                .userUpdated$
                .subscribe(user => {
                    const entry = this.users.find(u => u.id === user.id);
                    if (entry) {
                        const index = this.users.indexOf(entry);
                        this.users[index] = { ...user };
                    }
                }),
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof DirectMessagingListComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }

}
