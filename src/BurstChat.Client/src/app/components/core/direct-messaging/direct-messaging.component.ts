import { Component, computed } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faComments } from '@fortawesome/free-solid-svg-icons';
import { DisplayDirectMessages } from 'src/app/models/sidebar/display-direct-messages';
import { SidebarService } from 'src/app/services/sidebar/sidebar.service';

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
export class DirectMessagingComponent {

    public comments = faComments;

    public isActive = computed(() => {
        const options = this.sidebarService.display();
        return options instanceof DisplayDirectMessages;
    });

    /**
     * Creates an instance of DirectMessagingComponent.
     * @memberof DirectMessagingComponent
     */
    constructor(private sidebarService: SidebarService) { }

    /**
     * Handles the direct messaging button click event.
     * @memberof DirectMessagingComponent
     */
    public onDirectMessaging() {
        this.sidebarService.toggleDisplay(new DisplayDirectMessages());
    }

}
