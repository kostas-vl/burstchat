import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faComments } from '@fortawesome/free-solid-svg-icons';
import { DisplayDirectMessages } from 'src/app/models/sidebar/display-direct-messages';
import { SidebarService } from 'src/app/modules/burst/services/sidebar/sidebar.service';

/**
 * This class represents an angular component that displays the list direct messagin of a user.
 * @export
 * @class DirectMessagingComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-direct-messaging',
    templateUrl: './direct-messaging.component.html',
    styleUrl: './direct-messaging.component.scss',
    standalone: true,
    imports: [FontAwesomeModule]
})
export class DirectMessagingComponent implements OnInit, OnDestroy {

    private displaySub?: Subscription;

    public comments = faComments;

    public isActive = false;

    /**
     * Creates an instance of DirectMessagingComponent.
     * @memberof DirectMessagingComponent
     */
    constructor(private sidebarService: SidebarService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof DirectMessagingComponent
     */
    public ngOnInit() {
        this.displaySub = this
            .sidebarService
            .display
            .subscribe(options => {
                this.isActive = options instanceof DisplayDirectMessages;
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof DirectMessagingComponent
     */
    public ngOnDestroy() {
        if (this.displaySub) {
            this.displaySub.unsubscribe();
        }
    }

    /**
     * Handles the direct messaging button click event.
     * @memberof DirectMessagingComponent
     */
    public onDirectMessaging() {
        this.sidebarService.toggleDisplay(new DisplayDirectMessages());
    }

}
