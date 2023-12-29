import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { ChangePassword } from 'src/app/models/user/change-password';
import { tryParseError } from 'src/app/models/errors/error';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SessionService } from 'src/app/services/session/session.service';

/**
 * This class represents an angular component that enables the user to change his password.
 * @class ChangePasswordComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-change-password',
    templateUrl: './change-password.component.html',
    styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit, OnDestroy {

    public queryParamsSub?: Subscription;

    public changePassword = new ChangePassword();

    public dragon = faDragon;

    public loading = false;

    /**
     * Creates an instance of ChangePasswordComponent
     * @memberof ChangePasswordComponent
     */
    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private sessionService: SessionService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ChangePasswordComponent
     */
    public ngOnInit() {
        this.queryParamsSub = this
            .activatedRoute
            .queryParams
            .subscribe(params => {
                if ('email' in params) {
                    this.changePassword.email = params.email;
                } else {
                    this.router.navigateByUrl('/session/reset');
                }
            });
    }

    /**
     *
     * Executes any necessary code for the destruction of the component.
     * @memberof ChangePasswordComponent
     */
    public ngOnDestroy() {
        if (this.queryParamsSub) {
            this.queryParamsSub.unsubscribe();
        }
    }

    /**
     * Handles the change password click event.
     * @memberof ChangePasswordComponent
     */
    public onChangePassword() {
        this.loading = true;
        this.sessionService
            .changePassword(this.changePassword)
            .subscribe(
                () => {
                    const title = 'Your password changed successfully';
                    this.notifyService.popupSuccess(title, '');
                    this.router.navigateByUrl('/session/login');
                },
                error => {
                    let apiError = tryParseError(error.error);
                    apiError = apiError || {
                        level: 'warning',
                        type: 'Validation',
                        message: 'Please try to change your password in a few seconds.'
                    };
                    this.notifyService.popupError(apiError);
                    this.loading = false;
                }
            );
    }

}
