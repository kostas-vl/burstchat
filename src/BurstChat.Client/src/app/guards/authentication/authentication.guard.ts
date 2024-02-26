import { inject } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { StorageService } from 'src/app/services/storage/storage.service';

export const authenticationGuard: CanActivateFn = (_route, _state) => {
    const router = inject(Router);
    const storageService = inject(StorageService);
    return toObservable(storageService.tokenInfo)
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
