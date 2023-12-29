import { HttpClient, HttpHandlerFn, HttpHeaders, HttpInterceptorFn, HttpParams, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';
import { catchError, filter, flatMap, map, switchMap, take } from 'rxjs/operators';
import { TokenInfo } from 'src/app/models/identity/token-info';
import { StorageService } from 'src/app/services/storage/storage.service';
import { environment } from 'src/environments/environment';

/**
 * This method will request a new access token by using the stored refresh token.
 * @private
 * @param token {TokenInfo} the token info instance.
 * @returns A subscription to the post refresh token request.
 * @memberof AuthHttpInterceptor
 */
const requestRefresh = (router: Router, httpClient: HttpClient, storageService: StorageService, token: TokenInfo) => {
    storageService.setRefreshToken(null);
    storageService.refreshTokenLock = true;

    const body = new HttpParams()
        .set('client_id', environment.clientId)
        .set('client_secret', environment.clientSecret)
        .set('scope', environment.scope)
        .set('grant_type', environment.refreshTokenGrantType)
        .set('refresh_token', token.refreshToken);

    const httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/x-www-form-urlencoded'
        })
    };

    httpClient
        .post<any>('/connect/token', body.toString(), httpOptions)
        .subscribe(data => {
            if (data) {
                const info: TokenInfo = {
                    idToken: data.id_token || null,
                    accessToken: data.access_token,
                    expiresIn: data.expires_in,
                    refreshToken: data.refresh_token,
                    scope: data.scope,
                    tokenType: data.token_type
                };
                storageService.setTokenInfo(info);
                storageService.setRefreshToken(info);
                storageService.refreshTokenLock = false;
            }
        }, _ => {
            router.navigateByUrl('/session/logout');
        });
}


/**
 * This method will extend the provided request in order to add an access token to the Authorization
 * header.
 * @param token {TokenInfo} The token info instance.
 * @param request {HttpRequest(unknown)} The request to be extended.
 * @returns The modified HttpRequest.
 */
const requestWithAccessToken = (token: TokenInfo, request: HttpRequest<unknown>) => {
    const configuredRequest = request.clone({
        setHeaders: {
            Authorization: `Bearer ${token.accessToken}`
        }
    });

    return configuredRequest;
}

/**
 * This function with intercept an http request inorder to assign the appropriate bearer token or
 * refresh it based on the status code.
 * @param request {HttpRequest(unknown)} The current http request instance.
 * @param next {HttpHandlerFn} The next http interceptor handler function.
 * returns An observable with the appropriate http event.
 */
export const authInterceptor: HttpInterceptorFn = (request, next) => {
    const router = inject(Router);
    const httpClient = inject(HttpClient);
    const storageService = inject(StorageService);
    return storageService
        .tokenInfo
        .pipe(
            flatMap(token => {
                return next(requestWithAccessToken(token, request))
                    .pipe(
                        catchError(error => {
                            if (error?.status !== 401) {
                                return throwError(error);
                            }

                            if (!storageService.refreshTokenLock) {
                                requestRefresh(router, httpClient, storageService, token);
                            }

                            return storageService
                                .refreshToken$
                                .pipe(
                                    filter(info => info !== null),
                                    take(1),
                                    switchMap((t: TokenInfo) => next(requestWithAccessToken(t, request))),
                                    catchError(refreshError => {
                                        router.navigateByUrl('/session/logout');
                                        return throwError(refreshError);
                                    })
                                );
                        })
                    );
            })
        );
};
