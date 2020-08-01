import { Injectable } from '@angular/core';
import { Subject, BehaviorSubject } from 'rxjs';

/**
 * This class represents an angular service that toggles the chat layout between the text chat and the
 * voice call components.
 * @export
 * @class ChatLayoutService
 */
@Injectable()
export class ChatLayoutService {

    private toggleSource$ = new BehaviorSubject<'chat' | 'call'>('chat');

    public toggle$ = this.toggleSource$.asObservable();

    constructor() { }

    /**
     * Toggles to the next state of the layout based on the current value.
     * @memberof ChatLayoutService
     */
    public toggle(state: 'chat' | 'call') {
        this.toggleSource$.next(state);
    }

}
