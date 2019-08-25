import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { TokenInfo } from 'src/app/models/identity/token-info';

/**
 * This class represents an angular service that exposes functionality for storing data in a platform specific
 * storage.
 * @class StorageService
 */
@Injectable()
export class StorageService {

    /**
     * Creates an instance of StorageService.
     * @memberof StorageService
     */
    constructor() { }

    /**
     * Stores the provided token info to the appropriate platform storage and triggers a new
     * value to the service's observables.
     * @memberof StorageService
     * @param { TokenInfo } tokenInfo The information about the current tokens
     */
    public storeTokenInfo(tokenInfo: TokenInfo): void {
        if (tokenInfo) {
            const rawTokenInfo = JSON.stringify(tokenInfo);
            localStorage.setItem("tokenInfo", rawTokenInfo);
        }
    }

    /**
     * Fetches the value stored for the token info in the appropriate platform storage.
     * @memberof StorageService
     * @returns { TokenInfo | null } The TokenInfo instance or null if none is stored.
     */
    public getTokenInfo(): TokenInfo | null {
        const rawTokenInfo = localStorage.getItem("tokenInfo");
        return JSON.parse(rawTokenInfo) as TokenInfo | null;
    }

    /**
     * Clears any neccessary value from the appropriate platform storage.
     * @memberof StorageService
     */
    public clear(): void {
        localStorage.removeItem("tokenInfo");
    }

}

