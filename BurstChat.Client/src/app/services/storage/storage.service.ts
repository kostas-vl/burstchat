import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { TokenInfo } from 'src/app/models/identity/token-info';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';

/**
 * This class represents an angular service that exposes functionality for storing data in a platform specific
 * storage.
 * @class StorageService
 */
@Injectable()
export class StorageService {

    /**
     * Fetches the value stored for the token info in the appropriate platform storage.
     * @memberof StorageService
     * @returns { TokenInfo | null } The TokenInfo instance or null if none is stored.
     */
    public get tokenInfo(): TokenInfo | null {
        const rawTokenInfo = localStorage.getItem('tokenInfo');
        return JSON.parse(rawTokenInfo) as TokenInfo | null;
    }

    /**
     * Stores the provided token info to the appropriate platform storage.
     * @memberof StorageService
     * @param { TokenInfo } tokenInfo The information about the current tokens
     */
    public set tokenInfo(info: TokenInfo) {
        if (info) {
            const rawTokenInfo = JSON.stringify(info);
            localStorage.setItem('tokenInfo', rawTokenInfo);
        }
    }

    /**
     * Fetches the last active chat of the user that is stored in the appropriate platform storage.
     * @memberof StorageService
     * @returns { PrivateGroupConnectionOptions | ChannelConnectionOptions | null } The chat connection options found or null.
     */
    public get activeChat(): PrivateGroupConnectionOptions | ChannelConnectionOptions | null{
        const rawOptions = localStorage.getItem('activeChat');
        const isPrivateOptions = rawOptions.hasOwnProperty('privateGroupId');
        if (isPrivateOptions) {
            const options = Object.assign(new PrivateGroupConnectionOptions(), rawOptions);
            return options;
        }

        const isChannelOptions = rawOptions.hasOwnProperty('channelId');
        if (isChannelOptions) {
            const options = Object.assign(new ChannelConnectionOptions(), rawOptions);
            return options;
        }

        return null;
    }

    /**
     * Stores the provided active chat options to the appropriate platform storage.
     * @memberof StorageService
     * @param { PrivateGroupConnectionOptions | ChannelConnectionOptions } options The options of the active chat.
     */
    public set activeChat(options: PrivateGroupConnectionOptions | ChannelConnectionOptions) {
        const rawOptions = JSON.stringify(options);
        localStorage.setItem('activeChat', rawOptions);
    }

    /**
     * Creates an instance of StorageService.
     * @memberof StorageService
     */
    constructor() { }

    /**
     * Clears any neccessary value from the appropriate platform storage.
     * @memberof StorageService
     */
    public clear(): void {
        localStorage.removeItem('tokenInfo');
    }

}

