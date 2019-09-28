import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { User } from 'src/app/models/user/user';

/**
 * This class represents an angular service that exposes methods for fetching, subscribing and transforming BurstChat servers.
 * @class ServersService
 */
@Injectable()
export class ServersService {

    private activeServerSource = new BehaviorSubject<Server | null>(null);

    public activeServer = this.activeServerSource.asObservable();

    /**
     * Creates a new instance of ServersService.
     * @memberof ServersService
     */
    constructor(private httpClient: HttpClient) { }

    /**
     * Informs all observers of the active server about its new value.
     * @memberof ServersService
     */
    public setActiveServer(server: Server | null) {
        if (server) {
            this.activeServerSource
                .next(server);
        }
    }

    /**
     * Requests to the BurstChat API all information about a server based on the provided id and returns
     * an observable that will be invoked when the requests completes.
     * @memberof ServersService
     * @returns {Observable<Server>} The observable to be invoked.
     */
    public get(serverId: number): Observable<Server> {
        return this
            .httpClient
            .get<Server>(`/api/servers/${serverId}`);
    }

    /**
     * Requests the creation of a new server and returns an observable that will be invoke after the request
     * completes.
     * @memberof ServersService
     * @param {Server} server The server instance that will be added.
     * @returns An obserable.
     */
    public post(server: Server): Observable<{}> {
        return this
            .httpClient
            .post('/api/servers', server);
    }

    /**
     * Requests to update information about an existing server and returns an observable that will be invoked after
     * the request completes.
     * @param {Server} server The server instance to be updated.
     * @returns An obseravable.
     */
    public put(server: Server): Observable<{}> {
        return this
            .httpClient
            .put('/api/servers', server);
    }

    /**
     * Requests the deletion of an existing server and returns an observable that will be invoked after the request
     * completes.
     * @memberof ServersService
     * @param {number} serverId The id of the server to be deleted.
     * @returns An observable.
     */
    public delete(serverId: number): Observable<{}> {
        return this
            .httpClient
            .delete(`/api/servers/${serverId}`);
    }

    /**
     * Requests the subscribed users of a server based on the provided id and returns an observable that will be invoked after
     * the request completes.
     * @param {number} serverId The id of the target server.
     * @returns {Observable<User[]>} An observable containing an array of Users.
     * @memberof ServersService
     */
    public getSubscribedUsers(serverId: number): Observable<User[]> {
        return this
            .httpClient
            .get<User[]>(`/api/servers/${serverId}/subscribedUsers`);
    }

}

