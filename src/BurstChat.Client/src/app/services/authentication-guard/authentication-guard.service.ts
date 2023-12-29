import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router, CanActivateFn } from '@angular/router';
import {  map } from 'rxjs/operators';
import { StorageService } from 'src/app/services/storage/storage.service';

export const authenticationGuardFn: CanActivateFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const router = inject(Router);
    const storageService = inject(StorageService);
    return storageService
        .tokenInfo
        .pipe(
            map(info => {
                const isAllowed = info && info.accessToken !== null;

                if (!isAllowed) {
                    router.navigateByUrl('/session/login');
                }

                return isAllowed;
            })
        );
};
