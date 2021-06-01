import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { Message } from 'src/app/models/chat/message';

/**
 * This class represents an angular service that exposes methods for displaying various
 * dialogs associated with chat messages.
 * @export
 * @class ChatDialogService
 */
@Injectable()
export class ChatDialogService {

    private editMessageSource$ = new Subject<Message>();

    private deleteMessageSource$ = new Subject<Message>();

    public editMessage$ = this.editMessageSource$.asObservable();

    public deleteMessage$ = this.deleteMessageSource$.asObservable();

    /**
     * Displays a dialog for editing a chat message.
     * @param {Message} message The message to be edited.
     * @memberof ChatDialogService
     */
    public editMessage(message: Message) {
        this.editMessageSource$.next(message);
    }

    /**
     * Displays a dialog for deleting a chat message.
     * @param {Message} message The message to be deleted.
     * @memberof ChatDialogService
     */
    public deleteMessage(message: Message) {
        this.deleteMessageSource$.next(message);
    }

}
