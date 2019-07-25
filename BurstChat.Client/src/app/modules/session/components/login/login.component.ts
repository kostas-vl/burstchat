import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Credentials } from 'src/app/models/user/credentials';

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

    public loading = false;

    /**
     * Creates an instance of LoginComponent.
     * @memberof LoginComponent
     */
    constructor(private router: Router) { }

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
        setTimeout(() => this.router.navigateByUrl('/core/chat'), 3000);
    }

}
