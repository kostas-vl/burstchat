import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ChangePassword } from 'src/app/models/user/change-password';

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

    public loading = false;

    /**
     * Creates an instance of ChangePasswordComponent
     * @memberof ChangePasswordComponent
     */
    constructor(private router: Router) { }

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
        setTimeout(() => this.router.navigateByUrl('/session/login'), 3000);
    }

}
