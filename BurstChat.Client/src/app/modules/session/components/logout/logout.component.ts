import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

/**
 * This class represents an angular component that terminates the session of a user
 * and redirects him to the login page.
 * @class LogoutComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.scss']
})
export class LogoutComponent implements OnInit {

    /**
     * Create an instance of LogoutComponent.
     * @memberof LogoutComponent
     */
    constructor(private router: Router) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof LogoutComponent
     */
    public ngOnInit() {
        setTimeout(() => this.router.navigateByUrl('/session/login'), 3000);
    }

}
