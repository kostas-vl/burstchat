import { Injectable, WritableSignal, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { User } from 'src/app/models/user/user';

/**
 * This class represents an angular service that exposes methods for fetching, subscribing and transforming BurstChat servers.
 * @class ServersService
 */
@Injectable()
export class ServersService {

    private serverInfoSource: WritableSignal<Server | null> = signal(null);

    private serverCacheSource: WritableSignal<Server[]> = signal([]);

    public serverInfo = this.serverInfoSource.asReadonly();

    public serverCache = this.serverCacheSource.asReadonly();

    /**
     * Creates a new instance of ServersService.
     * @memberof ServersService
     */
    constructor(private httpClient: HttpClient) { }

    /**
     * Fetches the current value of the server info source.
     * @memberof ServersService
     */
    public current() {
        return this.serverInfoSource();
    }

    /**
     * Assigns the current focused server based on the provided id and the registered cache.
     * @param {number} serverId The id of the target server
     * @memberof ServersService
     */
    public set(serverId: number) {
        const servers = this.serverCacheSource();
        const server = servers.find(s => s.id === serverId);
        const invalidInfo = server
            && (server.channels?.length === 0 || server.subscriptions?.length === 0);

        if (invalidInfo) {
            this.get(serverId);
            return;
        }

        this.serverInfoSource.set(server);
    }

    /**
     * Requests to the BurstChat API all information about a server based on the provided id.
     * @memberof ServersService
     */
    public get(serverId: number) {
        this.httpClient
            .get<Server>(`/api/servers/${serverId}`)
            .subscribe(server => this.serverInfoSource.set(server));
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
    public put(server: Server) {
        return this
            .httpClient
            .put<Server>('/api/servers', server);
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

    /**
     * Updates the list of servers available to the user.
     * @param {Server[]} servers The list of server to replace the current cache.
     * @memberof ServersService
     */
    public updateCache(servers: Server[]) {
        this.serverCacheSource.set(servers);
    }

}

