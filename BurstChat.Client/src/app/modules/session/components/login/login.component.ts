import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { Credentials } from 'src/app/models/user/credentials';
import { tryParseError, BurstChatError } from 'src/app/models/errors/error';
import { TokenInfo } from 'src/app/models/identity/token-info';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { StorageService } from 'src/app/services/storage/storage.service';
import { SessionService } from 'src/app/modules/session/services/session-service/session.service';

/**
 * This class represents an angular component that displays the login page to the user.
 * @class LoginComponent
 * @memberof {OnInit}
 */
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

    public credentials = new Credentials();

    public dragon = faDragon;

    public loading = false;

    /**
     * Creates an instance of LoginComponent.
     * @memberof LoginComponent
     */
    constructor(
        private router: Router,
        private notifyService: NotifyService,
        private storageService: StorageService,
        private sessionService: SessionService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof LoginComponent
     */
    public ngOnInit() { }

    /**
     * Handles the click event of the login button.
     * @memberof LoginComponent
     */
    public onLogin(): void {
        this.loading = true;
        this.sessionService
            .validate(this.credentials)
            .subscribe(
                data => {
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
                        this.router.navigateByUrl('/core/home');
                    } else {
                        const error: BurstChatError = {
                            level: 'warning',
                            type: 'Validation',
                            message: 'Could not properly login, please retry in a few minutes'
                        };
                        this.notifyService.notifyError(error);
                    }
                },
                error => {
                    let apiError = tryParseError(error.error);
                    apiError = apiError || {
                        level: 'warning',
                        type: 'Validation',
                        message: 'Please try to login again in a few seconds.'
                    };
                    this.notifyService.notifyError(apiError);
                    this.loading = false;
                }
            );
    }

    /**
     * Handles the key down event of various input elements.
     * @param {KeyboardEvent} event The event arguments.
     * @memberof LoginComponent
     */
    public onKeyDown(event: KeyboardEvent) {
        if (event.key === 'Enter') {
            this.onLogin();
        }
    }

}
