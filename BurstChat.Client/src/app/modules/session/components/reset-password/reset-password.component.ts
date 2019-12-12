import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BurstChatError, tryParseError } from 'src/app/models/errors/error';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SessionService } from 'src/app/modules/session/services/session-service/session.service';

/**
 * This class represents an angular component that helps the user to reset his password.
 * @class ResetPasswordComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {

    public email?: string;

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
                () => this.router.navigateByUrl('/session/change'),
                error => {
                    console.log(error);
                    let apiError = tryParseError(error.error);
                    apiError = apiError || {
                        level: 'warning',
                        type: 'validation',
                        message: 'Please try to reset your password in a few seconds.'
                    };
                    this.notifyService.notifyError(apiError);
                    this.loading = false;
                }
            );
    }

}
