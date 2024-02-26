import { Injectable, WritableSignal, signal } from '@angular/core';
import { Message } from 'src/app/models/chat/message';

/**
 * This class represents an angular service for the chat modules that enables interaction
 * and data transfer between ui components.
 * @export
 */
@Injectable()
export class UiLayerService {

    private layoutSource: WritableSignal<'chat' | 'call'> = signal('chat');

    private editedMessageSource: WritableSignal<Message | null> = signal(null);

    private deleteMessageSource: WritableSignal<Message | null> = signal(null);

    private searchSource: WritableSignal<string | null> = signal(null);

    public layout = this.layoutSource.asReadonly();

    public editMessage = this.editedMessageSource.asReadonly();

    public deleteMessage = this.deleteMessageSource.asReadonly();

    public search = this.searchSource.asReadonly();

    /**
     * Creates a new instance of UiLayerService.
     * @memberof UiLayerService
     */
    constructor() { }

    /**
     * Changes the view of the chat layout based on the provided value.
     * @param {'chat' | 'call'} view The view state to be set.
     * @memberof UiLayerService
     */
    public changeLayout(view: 'chat' | 'call') {
        this.layoutSource.set(view);
    }

    /**
     * Displays a dialog for editing a chat message.
     * @param {Message} message The message to be edited.
     * @memberof UiLayerService
     */
    public edit(message: Message) {
        this.editedMessageSource.set(message);
    }

    /**
     * Displays a dialog for deleting a chat message.
     * @param {Message} message The message to be deleted.
     * @memberof UiLayerService
     */
    public delete(message: Message) {
        this.deleteMessageSource.set(message);
    }

    /**
     * Sets a search term that will filter the messages displayed on chat.
     * @param {string | null} term The search term to be searched.
     * @memberof UiLayerService
     */
    public searchTerm(term: string | null)  {
        this.searchSource.set(term);
    }
}
