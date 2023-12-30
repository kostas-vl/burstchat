import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { tryParseError } from 'src/app/models/errors/error';
import { Registration } from 'src/app/models/user/registration';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { SessionService } from 'src/app/services/session/session.service';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';

/**
 * This class represents an angular component that enables a user to register an account in BurstChat.
 * @class RegisterComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-register',
    templateUrl: './register.component.html',
    styleUrl: './register.component.scss',
    standalone: true,
    imports: [
        FormsModule,
        FontAwesomeModule,
        RouterLink,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
    ],
    providers: [SessionService]
})
export class RegisterComponent implements OnInit {

    public registration = new Registration();

    public dragon = faDragon;

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
    public ngOnInit() { }

    /**
     * Handles the register button click event.
     * @memberof RegisterComponent
     */
    public onRegister() {
        this.loading = true;
        this.sessionService
            .register(this.registration)
            .subscribe(
                () => {
                    const title = 'Successful registration';
                    const message = 'Thank you for registering an account on BurstChat. Use your credentials to login onto the application';
                    this.notifyService.popupSuccess(title, message);
                    this.router.navigateByUrl('/session/login')
                },
                httpError => {
                    const error = tryParseError(httpError.error);
                    this.notifyService.popupError(error);
                    this.loading = false;
                }
            );
    }

}
