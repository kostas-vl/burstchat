import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { ChangePassword } from 'src/app/models/user/change-password';
import { tryParseError } from 'src/app/models/errors/error';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SessionService } from 'src/app/modules/session/services/session-service/session.service';

/**
 * This class represents an angular component that enables the user to change his password.
 * @class ChangePasswordComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-change-password',
    templateUrl: './change-password.component.html',
    styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {

    public changePassword = new ChangePassword();

    public dragon = faDragon;

    public loading = false;

    /**
     * Creates an instance of ChangePasswordComponent
     * @memberof ChangePasswordComponent
     */
    constructor(
        private router: Router,
        private notifyService: NotifyService,
        private sessionService: SessionService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChangePasswordComponent
     */
    public ngOnInit(): void { }

    /**
     * Handles the change password click event.
     * @memberof ChangePasswordComponent
     */
    public onChangePassword(): void {
        this.loading = true;
        this.sessionService
            .changePassword(this.changePassword)
            .subscribe(
                () => {
                    const title = 'Your password changed successfully';
                    this.notifyService.notify(title);
                    this.router.navigateByUrl('/session/login');
                },
                error => {
                    let apiError = tryParseError(error.error);
                    apiError = apiError || {
                        level: 'warning',
                        type: 'Validation',
                        message: 'Please try to change your password in a few seconds.'
                    };
                    this.notifyService.notifyError(apiError);
                    this.loading = false;
                }
            );
    }

}
