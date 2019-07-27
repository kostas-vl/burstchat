import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BurstChatError, tryParseError } from 'src/app/models/errors/error';
import { Registration } from 'src/app/models/user/registration';
import { Notification } from 'src/app/models/notify/notification';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SessionService } from 'src/app/modules/session/services/session-service/session.service';

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
    constructor(
        private router: Router,
        private notifyService: NotifyService,
        private sessionService: SessionService
    ) { }

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
        this.sessionService
            .register(this.registration)
            .subscribe(
                () => this.router.navigateByUrl('/session/login'),
                error => {
                    const apiError = tryParseError(error.error);
                    const notification: Notification = {
                        title: 'An error occured',
                        content: apiError !== null
                            ? apiError.message
                            : 'Please try to register again in a few seconds.'
                    };
                    this.notifyService.notify(notification);
                    this.loading = false;
                }
            );
    }

}
