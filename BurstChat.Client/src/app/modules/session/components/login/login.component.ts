import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Credentials } from 'src/app/models/user/credentials';
import { BurstChatError, tryParseError } from 'src/app/models/errors/error';
import { Notification } from 'src/app/models/notify/notification';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SessionService } from 'src/app/modules/session/services/session-service/session.service';

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
    constructor(
        private router: Router,
        private notifyService: NotifyService,
        private sessionService: SessionService
    ) { }

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
        this.sessionService
            .validate(this.credentials)
            .subscribe(
                () => this.router.navigateByUrl('/core/chat'),
                error => {
                  const apiError = tryParseError(error.error);
                  const notification: Notification = {
                      title: 'An error occured',
                      content: apiError !== null
                          ? apiError.message
                          : 'Please try to login again in a few seconds.'
                  };
                  this.notifyService.notify(notification);
                  this.loading = false;
                }
            );
    }

}
