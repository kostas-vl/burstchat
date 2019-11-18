import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { StorageService } from 'src/app/services/storage/storage.service';

/**
 * This class represents an angular service that filters the access to routes by checking if there is data
 * about an authenticated user.
 * @class AuthenticationGuardService
 */
@Injectable()
export class AuthenticationGuardService implements CanActivate {

    /**
     * Creates a new instance of AuthenticationGuardService
     * @memberof AuthenticationGuardService
     */
    constructor(
        private router: Router,
        private storageService: StorageService
    ) { }

    /**
     * This method returns a boolean that reflects whether a route can be accessed based on if there is
     * data about an authenticated user.
     * @memberof AuthenticationGuardService
     * @param { ActivatedRouteSnapshot } next The activated route snapshot.
     * @param { RouterStateSnapshot } state The router state snapshot.
     * @returns { boolean } The boolean that specifies whether a route can be accessed.
     */
    public canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        const tokenInfo = this
            .storageService
            .tokenInfo;
        const isAllowed = tokenInfo !== null && tokenInfo.accessToken !== null;

        if (!isAllowed) {
            this.router.navigateByUrl('/session/login');
        }

        return isAllowed;
    }

}

