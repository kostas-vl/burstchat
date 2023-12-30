import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { Message } from 'src/app/models/chat/message';

/**
 * This class represents an angular service for the chat modules that enables interaction
 * and data transfer between ui components.
 * @export
 */
@Injectable()
export class UiLayerService {

    private toggleChatViewSource$ = new BehaviorSubject<'chat' | 'call'>('chat');

    private editMessageSource$ = new Subject<Message>();

    private deleteMessageSource$ = new Subject<Message>();

    private searchSource$ = new BehaviorSubject<string | null>(null);

    public toggleChatView$ = this.toggleChatViewSource$.asObservable();

    public editMessage$ = this.editMessageSource$.asObservable();

    public deleteMessage$ = this.deleteMessageSource$.asObservable();

    public search$ = this.searchSource$.asObservable();

    /**
     * Creates a new instance of UiLayerService.
     * @memberof UiLayerService
     */
    constructor() { }

    /**
     * Toggles the view of the chat layout based on the provided value.
     * @param {'chat' | 'call'} view The view state to be set.
     * @memberof UiLayerService
     */
    public toggleChatView(view: 'chat' | 'call') {
        this.toggleChatViewSource$.next(view);
    }

    /**
     * Displays a dialog for editing a chat message.
     * @param {Message} message The message to be edited.
     * @memberof UiLayerService
     */
    public editMessage(message: Message) {
        this.editMessageSource$.next(message);
    }

    /**
     * Displays a dialog for deleting a chat message.
     * @param {Message} message The message to be deleted.
     * @memberof UiLayerService
     */
    public deleteMessage(message: Message) {
        this.deleteMessageSource$.next(message);
    }

    /**
     * Sets a search term that will filter the messages displayed on chat.
     * @param {string | null} term The search term to be searched.
     * @memberof UiLayerService
     */
    public search(term: string | null)  {
        this.searchSource$.next(term);
    }
}
