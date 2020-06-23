import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { User } from 'src/app/models/user/user';
import { Server } from 'src/app/models/servers/server';

@Injectable()
/**
 * This class represents an angular service the exposes method for fetching and updating data of an authenticated user.
 * @class UserService
 */
export class UserService {

    private userSource = new BehaviorSubject<User | null>(null);

    private subscriptionsSource = new BehaviorSubject<Server[]>([]);

    private usersCacheSource = new BehaviorSubject<{ [id: string]: User[] }>({});

    public user = this.userSource.asObservable();

    public subscriptions = this.subscriptionsSource.asObservable();

    public usersCache = this.usersCacheSource.asObservable();

    /**
     * Creates a new instance of UserService.
     * @memberof UserService
     */
    constructor(private httpClient: HttpClient) { }

    /**
     * Requests information about the user from the BurstChat API.
     * @memberof UserService
     */
    public getUser() {
        this.httpClient
            .get<User>('/api/user')
            .subscribe(data => this.userSource.next(data));
    }

    /**
     * Requests the subscribed server of the current authenticate user.
     * @memberof UserService
     */
    public getSubscriptions() {
        this.httpClient
            .get<Server[]>('/api/user/subscriptions')
            .subscribe(data => this.subscriptionsSource.next(data));
    }

    /**
     * Requests information about a user from the BurstChat API.
     * @param {number} userId The id of the target user.
     * @memberof UserService
     */
    public whoIs(userId: number) {
        return this
            .httpClient
            .get<User>(`/api/user/${userId}`);
    }

    /**
     * Inserts a new pair of server id and users to the cache or updates an existing pair.
     * After the insert all observers of the cache are informed.
     * @param {number} serverId The id of the server.
     * @param {User[]} users The list of users.
     * @memberof UserService
     */
    public pushToCache(serverId: number, users: User[]) {
        const cache = this.usersCacheSource.getValue();
        cache[serverId.toString()] = users;
        this.usersCacheSource.next(cache);
    }

}
