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

    public userObservable = this.userSource.asObservable();

    public subscriptionsObservable = this.subscriptionsSource.asObservable();

    /**
     * Creates a new instance of UserService.
     * @memberof UserService
     */
    constructor(private httpClient: HttpClient) { }

    /**
     * Requests information about the user from the BurstChat API.
     * @memberof UserService
     */
    public getUser(): void {
        this.httpClient
            .get<User>('/api/user')
            .subscribe(data => this.userSource.next(data));
    }

    /**
     * Requests the subscribed server of the current authenticate user.
     * @memberof UserService
     */
    public getSubscriptions(): void {
        this.httpClient
            .get<Server[]>('/api/user/subscriptions')
            .subscribe(data => this.subscriptionsSource.next(data));
    }

}
