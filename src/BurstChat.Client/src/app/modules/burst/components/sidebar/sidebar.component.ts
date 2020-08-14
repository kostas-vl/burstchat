import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { SidebarService } from 'src/app/modules/burst/services/sidebar/sidebar.service';
import { DisplayServer } from 'src/app/models/sidebar/display-server';

/**
 * This class represents an angular component that displays on screen the main sidebar of the application.
 * @export
 * @class SidebarComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit, OnDestroy {

    private displaySub?: Subscription;

    public display?: 'direct' | 'server';

    /**
     * Creates an instance of SidebarComponent.
     * @memberof SidebarComponent
     */
    constructor(private sidebarService: SidebarService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof SidebarComponent
     */
    public ngOnInit() {
        this.displaySub = this
            .sidebarService
            .display
            .subscribe(options => {
                if (options instanceof DisplayServer) {
                    this.display = 'server';
                } else {
                    this.display = 'direct';
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof SidebarComponent
     */
    public ngOnDestroy() {
        if (this.displaySub) {
            this.displaySub.unsubscribe();
        }
    }

}
