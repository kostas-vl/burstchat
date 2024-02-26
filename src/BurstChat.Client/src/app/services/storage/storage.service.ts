import { Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { TokenInfo } from 'src/app/models/identity/token-info';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';

/**
 * This class represents an angular service that exposes functionality for storing data in a platform specific
 * storage.
 * @class StorageService
 */
@Injectable({
    providedIn: 'root'
})
export class StorageService {

    private refreshTokenSource = signal(null);

    private tokenInfoSource: WritableSignal<TokenInfo | null>;

    public refreshToken$ = this.refreshTokenSource.asReadonly();

    public refreshTokenLock = false;

    public tokenInfo: Signal<TokenInfo | null>;

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
    constructor() {
        const rawTokenInfo = localStorage.getItem('tokenInfo');
        const info = JSON.parse(rawTokenInfo) as TokenInfo | null;
        this.tokenInfoSource = signal(info);
        this.tokenInfo = this.tokenInfoSource.asReadonly();
    }

    /**
     * Stores the provided token info to the appropriate platform storage.
     * @memberof StorageService
     * @param tokenInfo { TokenInfo } The information about the current tokens
     */
    public setTokenInfo(info: TokenInfo) {
        if (info) {
            const rawTokenInfo = JSON.stringify(info);
            localStorage.setItem('tokenInfo', rawTokenInfo);
            this.tokenInfoSource.set(info);
        }
    }

    /**
     * Updates the value of the refresh token subject.
     * @param value {any} The value of the refresh token.
     */
    public setRefreshToken(value: any) {
        this.refreshTokenSource.set(value);
    }

    /**
     * Clears any neccessary value from the appropriate platform storage.
     * @memberof StorageService
     */
    public clear(): void {
        localStorage.removeItem('tokenInfo');
    }

}

