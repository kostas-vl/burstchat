import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TokenInfo } from 'src/app/models/identity/token-info';
import { Credentials } from 'src/app/models/user/credentials';
import { tryParseError, BurstChatError } from 'src/app/models/errors/error';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { SessionService } from 'src/app/services/session/session.service';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { StorageService } from 'src/app/services/storage/storage.service';

/**
 * This class represents an angular component that displays the login page to the user.
 * @class LoginComponent
 * @memberof {OnInit}
 */
@Component({
    selector: 'burst-login',
    templateUrl: './login.component.html',
    styleUrl: './login.component.scss',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        RouterLink,
        FontAwesomeModule,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
    ],
    providers: [SessionService]
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
                        this.notifyService.popupError(error);
                    }
                },
                error => {
                    let apiError = tryParseError(error.error);
                    apiError = apiError || {
                        level: 'warning',
                        type: 'Validation',
                        message: 'Please try to login again in a few seconds.'
                    };
                    this.notifyService.popupError(apiError);
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
