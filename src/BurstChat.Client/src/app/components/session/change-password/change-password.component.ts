import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { ChangePassword } from 'src/app/models/user/change-password';
import { tryParseError } from 'src/app/models/errors/error';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SessionService } from 'src/app/services/session/session.service';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';

/**
 * This class represents an angular component that enables the user to change his password.
 * @class ChangePasswordComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-change-password',
    templateUrl: './change-password.component.html',
    styleUrl: './change-password.component.scss',
    standalone: true,
    imports: [
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
