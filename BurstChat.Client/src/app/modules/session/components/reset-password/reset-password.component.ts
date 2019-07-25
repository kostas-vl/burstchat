import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

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
    constructor(private router: Router) { }

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
        setTimeout(() => this.router.navigateByUrl('/session/change'), 3000);
    }

}
