import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { tryParseError } from 'src/app/models/errors/error';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SessionService } from 'src/app/services/session/session.service';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';

/**
 * This class represents an angular component that helps the user to reset his password.
 * @class ResetPasswordComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-reset-password',
    templateUrl: './reset-password.component.html',
    styleUrl: './reset-password.component.scss',
    standalone: true,
    imports: [
        FormsModule,
        FontAwesomeModule,
        RouterLink,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
    ],
    providers: [SessionService]
})
export class ResetPasswordComponent implements OnInit {

    public email?: string;

    public dragon = faDragon;

    public loading = false;

    /**
     * Creates an instance of ResetPasswordComponent
     * @memberof ResetPasswordComponent
     */
    constructor(
        private router: Router,
        private notifyService: NotifyService,
        private sessionService: SessionService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ResetPasswordComponent
     */
    public ngOnInit(): void { }

    /**
     * Handles the reset password click event.
     * @memberof ResetPasswordComponent
     */
    public onResetPassword(): void {
        this.loading = true;
        this.sessionService
            .resetPassword(this.email)
            .subscribe(
                () => this.router.navigate(['/session/change'], {
                    queryParams: {
                        email: this.email
                    }
                }),
                error => {
                    console.log(error);
                    let apiError = tryParseError(error.error);
                    apiError = apiError || {
                        level: 'warning',
                        type: 'validation',
                        message: 'Please try to reset your password in a few seconds.'
                    };
                    this.notifyService.popupError(apiError);
                    this.loading = false;
                }
            );
    }

    /**
     *
     * Handles the email input's key down event.
     * @param {KeyboardEvent} event The event arguments.
     * @memberof ResetPasswordComponent
     */
    public onKeyDown(event: KeyboardEvent) {
        if (event.key === 'Enter') {
            this.onResetPassword();
        }
    }

}
