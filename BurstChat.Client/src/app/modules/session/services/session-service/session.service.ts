import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from  '@angular/common/http';
import { Observable } from 'rxjs';
import { Credentials } from 'src/app/models/user/credentials';
import { Registration } from 'src/app/models/user/registration';
import { ChangePassword } from 'src/app/models/user/change-password';

/**
 * This class represents an angular service that exposes methods for intercting with the
 * BurstChat's API endpoints for various user operations.
 * @class SessionService
 */
@Injectable()
export class SessionService {

    /**
     * Creates an instance of SessionService.
     * @memberof SessionService
     */
    constructor(private httpClient: HttpClient) { }

    /**
     * Creates the context of a registration request to the BurstChat API.
     * A subscription can be performed in order for the request to be sent and
     * the registration to be executed.
     * @memberof SessionService
     * @returns {Observable} An observable instance.
     */
    public register(data: Registration): Observable<{}> {
        return this
            .httpClient
            .post('/connect/register', data);
    }

    /**
     * Creates the context of a validation request for the BurstChat API.
     * A subscription can be performed in order for the request to be sent and
     * the validation to be executed.
     * @memberof SessionService
     * @returns {Observable} An observable instance.
     */
    public validate(data: Credentials): Observable<any> {
        const body = new HttpParams()
            .set("client_id", "burstchat.web.client")
            .set("client_secret", "secret")
            .set("scope", "openid profile offline_access burstchat.api burstchat.signal")
            .set("grant_type", "password")
            .set("username", data.email)
            .set("password", data.password);

        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/x-www-form-urlencoded'
            })
        };

        return this
            .httpClient
            .post('/connect/token', body.toString(), httpOptions);
    }

    /**
     * Creates the context of a password reset request for the BurstChat API.
     * A subscription can be performed in order for the request to be sent and
     * the password reset to be executed.
     * @memberof SessionService
     * @returns {Observable} An observable instance.
     */
    public resetPassword(email: string): Observable<{}> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        };
        return this
            .httpClient
            .post('/connect/password/reset', '"' + email + '"', httpOptions);
    }

    /**
     * Creates the context of a password change request for the BurstChat API.
     * A subscription can be performed in order for the request to be sent and
     * the password change to be executed.
     * @memberof SessionService
     * @returns {Observable} An observable instance.
     */
    public changePassword(data: ChangePassword): Observable<{}> {
        return this
            .httpClient
            .post('/connect/password/change', data);
    }

}
