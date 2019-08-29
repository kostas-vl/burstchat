import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Server } from 'src/app/models/servers/server';

/**
 * This class represents an angular service that exposes methods for fetching, subscribing and transforming BurstChat servers.
 * @class ServersService
 */
@Injectable()
export class ServersService {

    private serverSource = new BehaviorSubject<Server | null>(null);

    public serverObservable = this.serverSource.asObservable();

    /**
     * Creates a new instance of ServersService.
     * @memberof ServersService
     */
    constructor(private httpClient: HttpClient) { }

    /**
     * Request information about an existing server and invokes the proper subject in order to
     * inform any observers.
     * @memberof ServersService
     * @param {number} id The id of the target server.
     */
    public getServer(id: number): void {
        this.httpClient
            .get<Server>(`/api/servers/${id}`)
            .subscribe(server => {
                this.serverSource
                    .next(server);
            });
    }

    /**
     * Requests the creation of a new server and returns an observable that will be invoke after the request
     * completes.
     * @memberof ServersService
     * @param {Server} server The server instance that will be added.
     * @returns An obserable.
     */
    public postServer(server: Server): Observable<{}> {
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
    public putServer(server: Server): Observable<{}> {
        return this
            .httpClient
            .put('/api/servers', server);
    }

    /**
     * Requests the deletion of an existing server and returns an observable that will be invoked after the request
     * completes.
     * @memberof ServersService
     * @param {Server} server The server instance to be deleted.
     * @returns An observable.
     */
    public deleteServer(server: Server): Observable<{}> {
        return this
            .httpClient
            .delete(`/api/servers/${server.id}`);
    }

}

