import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Channel } from 'src/app/models/servers/channel';

/**
 * This class represents an angular service that exposes methods for fetching and transforming
 * BurstChat server channels.
 * @class ChannelsService
 */
@Injectable()
export class ChannelsService {

    /**
     * Creates a new instance of ChannelsService
     * @memberof ChannelsService
     */
    constructor(private httpClient: HttpClient) { }

    /**
     * Executes a request to the BurstChat API and returns an observable that will be invoked
     * when the request completes and returns information about the target channel.
     * @memberof ChannelsService
     * @param {number} channelId The id of the target channel.
     * @returns {Observable<Channel>} The observable that will be invoked.
     */
    public get(channelId: number): Observable<Channel> {
        return this
            .httpClient
            .get<Channel>(`/api/channels/${channelId}`);
    }

    /**
     * Executes a request to the BurstChat API that will create a new server channel and returns an observable
     * that will be invoked when the request completes.
     * @memberof ChannelsService
     * @param {number} serverId The id of the server the channel will be added.
     * @param {Channel} channel The channel information.
     * @returns {Observable<{}>} The observable that will be invoked.
     */
    public post(serverId: number, channel: Channel): Observable<{}> {
        const queryParams = new HttpParams();
        queryParams.set('serverId', serverId.toString());

        return this
            .httpClient
            .post(`/api/channels`, channel, { params: queryParams });
    }

    /**
     * Executes a request to the BurstChat API that will update information about a server channel and returns
     * an observable that will be invoked when the request completes.
     * @memberof ChannelsService
     * @param {Channel} channel The channel information that will be used in the update.
     * @returns {Observable<{}>} The observable that will be invoked.
     */
    public put(channel: Channel): Observable<{}> {
        return this
            .httpClient
            .put('/api/channels', channel);
    }

    /**
     * Executes a request to the BurstChat API that will remove all information about a channel and returns
     * an observable that will be invoked when the request completes.
     * @memberof ChannelsService
     * @param {number} channelId The id of the target channel.
     * @returns {Observable<{}>} The observable that will be invoked.
     */
    public delete(channelId: number): Observable<{}> {
        return this
            .httpClient
            .delete(`/api/channels/${channelId}`);
    }

}
