import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { TokenInfo } from 'src/app/models/identity/token-info';
import { StorageService } from 'src/app/services/storage/storage.service';

/**
 * This class represents an angular http interceptor that will assign a bearer token to an http request.
 * @class AuthHttpInterceptor
 */
@Injectable()
export class AuthHttpInterceptor implements HttpInterceptor {

    private refreshingToken = false;

    private tokenInfo?: TokenInfo;

    /**
     * Creates a new instance of AuthHttpInterceptor.
     * @memberof AuthHttpInterceptor
     */
    constructor(
        private router: Router,
        private httpClient: HttpClient,
        private storageService: StorageService
    ) {
        this.storageService
            .tokenInfo
            .subscribe(info => {
                if (info) {
                    this.tokenInfo = info;
                }
            });
    }

    /**
     * This method will request a new access token by using the stored refresh token.
     * @private
     * @returns A subscription to the post refresh token request.
     * @memberof AuthHttpInterceptor
     */
    private refresh() {
        const body = new HttpParams()
            .set('client_id', environment.clientId)
            .set('client_secret', environment.clientSecret)
            .set('scope', environment.scope)
            .set('grant_type', environment.refreshTokenGrantType)
            .set('refresh_token', this.tokenInfo.refreshToken);

        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/x-www-form-urlencoded'
            })
        };

        return this
            .httpClient
            .post<any>('/connect/token', body.toString(), httpOptions)
            .pipe(
                tap(data => {
                    if (data) {
                        const info: TokenInfo = {
                            idToken: data.id_token || null,
                            accessToken: data.access_token,
                            expiresIn: data.expires_in,
                            refreshToken: data.refresh_token,
                            scope: data.scope,
                            tokenType: data.token_type
                        };
                        this.storageService.setTokenInfo(info);
                    }
                })
            );
    }

    /**
     * This method will extend the provided request in order to add an access token to the Authorization
     * header.
     * @private
     * @memberof AuthHttpInterceptor
     * @param {HttpRequest(any)} request The request to be extended.
     * @returns The modified HttpRequest.
     */
    private requestWithAccessToken(request: HttpRequest<any>) {
        const accessToken = this
            .tokenInfo
            .accessToken;

        const configuredRequest = request.clone({
            setHeaders: {
                Authorization: `Bearer ${accessToken}`
            }
        });

        return configuredRequest;
    }

    /**
     * This method will assign a new header to an http request that will assign a bearer token.
     * @memberof AuthHttpInterceptor
     * @param {HttpRequest(any)} request The request instance.
     * @param {HttpHandler} next The http handler instance.
     * @return An observable that will conclude the request.
     */
    public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (this.tokenInfo) {
            return next
                .handle(this.requestWithAccessToken(request))
                .pipe(
                    catchError(error => {
                        if (error && error.status === 401) {
                            return this
                                .refresh()
                                .pipe(
                                    switchMap(_ => next.handle(this.requestWithAccessToken(request))),
                                    catchError(refreshError => {
                                        this.router.navigateByUrl('/session/logout');
                                        return throwError(refreshError);
                                    })
                                );
                        }

                        return throwError(error);
                    })
                );
        }

        return next.handle(request);
    }

}
