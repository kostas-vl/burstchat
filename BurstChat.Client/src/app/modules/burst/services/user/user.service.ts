import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

@Injectable()
/**
 * This class represents an angular service the exposes method for fetching and updating data of an authenticated user.
 * @class UserService
 */
export class UserService {

    private userSource = new BehaviorSubject<any>(null);

    public userObservable = this.userSource.asObservable();

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
            .get<any>('/connect/userinfo')
            .subscribe(data => {
                console.log(data);
            });
    }

}
