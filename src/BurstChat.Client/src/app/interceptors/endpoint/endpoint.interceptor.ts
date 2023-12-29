import { HttpInterceptorFn } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

export const endpointInterceptor: HttpInterceptorFn = (request, next) => {
    const isApi = request.url.startsWith('/api') || request.url.startsWith('api');
    const isIdentity = request.url.startsWith('/connect') || request.url.startsWith('connect');
    let clone = request;

    if (isApi) {
        clone = request.clone({
            url: `${environment.apiUrl}${request.url}`
        });
    }

    if (isIdentity) {
        clone = request.clone({
            url: `${environment.identityUrl}${request.url}`
        });
    }

    return next(clone).pipe(
        catchError(value => {
             return throwError(value);
        })
    );
};
