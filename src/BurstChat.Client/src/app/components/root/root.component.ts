import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { PopupListComponent } from 'src/app/components/shared/popup-list/popup-list.component';

/**
 * This class represents an angular component that is the primary host component for all other displayed components.
 * @export
 * @class RootComponent
 */
@Component({
    selector: 'burst-root',
    templateUrl: './root.component.html',
    styleUrl: './root.component.scss',
    standalone: true,
    imports: [
        RouterOutlet,
        PopupListComponent
    ]
})
export class RootComponent implements OnInit {
    /**
     * Creates an instance of RootComponent.
     * @memberof RootComponent
     */
    constructor(private notifyService: NotifyService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof RootComponent
     */
    public ngOnInit() {
        this.notifyService.permission();
    }

}
