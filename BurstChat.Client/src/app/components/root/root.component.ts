import { Component, OnInit } from '@angular/core';
import { NotifyService } from 'src/app/services/notify/notify.service';

/**
 * This class represents an angular component that is the primary host component for all other displayed components.
 * @export
 * @class RootComponent
 */
@Component({
    selector: 'app-root',
    templateUrl: './root.component.html',
    styleUrls: ['./root.component.scss']
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
