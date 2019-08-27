import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { StorageService } from 'src/app/services/storage/storage.service';

/**
 * This class represents an angular http interceptor that will assign a bearer token to an http request.
 * @class AuthHttpInterceptor
 */
@Injectable()
export class AuthHttpInterceptor implements HttpInterceptor {

    /**
     * Creates a new instance of AuthHttpInterceptor.
     * @memberof AuthHttpInterceptor
     */
    constructor(
        private router: Router,
        private storageService: StorageService
    ) { }

    /**
     * This method will assign a new header to an http request that will assign a bearer token.
     * @memberof AuthHttpInterceptor
     * @param {HttpRequest(any)} request The request instance.
     * @param {HttpHandler} next The http handler instance.
     * @return An observable that will conclude the request.
     */
    public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const accessToken = this
            .storageService
            .tokenInfo
            .accessToken;

        const configuredRequest = request.clone({
            headers: request.headers.set('Authorization', `Bearer ${accessToken}`)
        });

        return next
            .handle(configuredRequest)
            .pipe(catchError(value => {
                if (value && value.status === 401) {
                    this.router
                        .navigateByUrl('/session/login');
                }
                return of(value);
            }));
    }

}
