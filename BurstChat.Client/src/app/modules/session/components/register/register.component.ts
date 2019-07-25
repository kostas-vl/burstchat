import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Registration } from 'src/app/models/user/registration';

/**
 * This class represents an angular component that enables a user to register an account in BurstChat.
 * @class RegisterComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

    public registration = new Registration();

    public loading = false;

    /**
     * Creates an instance of RegisterComponent.
     * @memberof RegisterComponent
     */
    constructor(private router: Router) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof RegisterComponent
     */
    public ngOnInit(): void { }

    /**
     * Handles the register button click event.
     * @memberof RegisterComponent
     */
    public onRegister(): void {
        this.loading = true;
        setTimeout(() => this.router.navigateByUrl('/session/login'), 3000);
    }

}
