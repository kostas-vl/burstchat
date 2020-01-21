import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { StorageService } from 'src/app/services/storage/storage.service';
import { environment } from 'src/environments/environment';

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
        private httpClient: HttpClient,
        private storageService: StorageService
    ) { }

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
            .set('refresh_token', this.storageService.tokenInfo.refreshToken);

        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/x-www-form-urlencoded'
            })
        };

        return this
            .httpClient
            .post<any>('/connect/token', body.toString(), httpOptions)
            .subscribe(data => {
                if (data) {
                    this.storageService.tokenInfo = {
                        idToken: data.id_token || null,
                        accessToken: data.access_token,
                        expiresIn: data.expires_in,
                        refreshToken: data.refresh_token,
                        scope: data.scope,
                        tokenType: data.token_type
                    };
                }
            });
    }

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
            setHeaders: {
                Authorization: `Bearer ${accessToken}`
            }
        });

        return next
            .handle(configuredRequest)
            .pipe(
                catchError(value => {
                    if (value && value.status === 401) {
                        this.router
                            .navigateByUrl('/session/login');
                    }
                    return throwError(value);
                }
                ));
    }

}
