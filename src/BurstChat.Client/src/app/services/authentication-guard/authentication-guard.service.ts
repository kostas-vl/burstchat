import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router, CanActivateFn } from '@angular/router';
import { StorageService } from 'src/app/services/storage/storage.service';

export const authenticationGuardFn: CanActivateFn = async (_next: ActivatedRouteSnapshot, _state: RouterStateSnapshot) => {
    const router = inject(Router);
    const storageService = inject(StorageService);
    const tokenInfo = await storageService.tokenInfo.toPromise();
    const isAllowed = tokenInfo && tokenInfo.accessToken !== null;

    if (!isAllowed) {
        router.navigateByUrl('/session/login');
    }

    return isAllowed;
};
