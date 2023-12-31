import { Injectable, signal, WritableSignal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { DirectMessaging } from 'src/app/models/user/direct-messaging';
import { User } from 'src/app/models/user/user';

/**
 * This class represents an angular service that exposes methods for fetching and transforming direct messaging data.
 * @export
 * @class DirectMessagingService
 */
@Injectable()
export class DirectMessagingService {

    private usersSource: WritableSignal<User[]> = signal([]);

    public users = this.usersSource.asReadonly();

    /**
     * Creates an instance of DirectMessagingService.
     * @memberof DirectMessagingService
     */
    constructor(private httpClient: HttpClient) { }

    /**
     * Fetches all direct messaging entries that the user is part of. If the request to the
     * API is successfull the directMessagingList observers will be notified.
     * @returns An observable of DirectMessaging
     * @memberof DirectMessagingService
     */
    public get(firstParticipantId: number, secondParticipantId: number) {
        const params = new HttpParams()
            .set('firstParticipantId', firstParticipantId.toString())
            .set('secondParticipantId', secondParticipantId.toString());

        return this
            .httpClient
            .get<DirectMessaging>('/api/direct', { params });
    }

    /**
     * Fetches all users that the authenticates user has conversed in direct messages.
     * @memberof DirectMessagingService
     */
    public getUsers() {
        this.httpClient
            .get<User[]>('/api/direct/users')
            .subscribe(data => {
                if (data && data.length > 0) {
                    this.usersSource.set(data);
                }
            });
    }

}
