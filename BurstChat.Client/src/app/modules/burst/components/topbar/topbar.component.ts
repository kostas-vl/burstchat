import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { Server } from 'src/app/models/servers/server';
import { UserService } from 'src/app/modules/burst/services/user/user.service';

/**
 * This class represents an angular component that displays on screen the top bar of the application
 * that provides various types of functionality and actions.
 * @class TopbarComponent
 */
@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent implements OnInit, OnDestroy {

    private userSubscriptions?: Subscription;

    public servers?: Server[];

    /**
     * Creates a new instance of TopbarComponent.
     * @memberof TopbarComponent
     */
    constructor(
        private router: Router,
        private userService: UserService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof TopbarComponent
     */
    public ngOnInit(): void { 
        this.userSubscriptions = this
            .userService
            .subscriptionsObservable
            .subscribe(servers => this.servers = servers);
    }

    /*
     * Executes any neccessary code for the destruction of the component.
     * @memberof TopbarComponent
     */
    public ngOnDestroy(): void {
        if (this.userSubscriptions) {
            this.userSubscriptions
                .unsubscribe();
        }
    }

    /*
     * Handles the logout button click event.
     * @memberof TopbarComponent
     */
    public onLogout(): void {
        this.router.navigateByUrl('/session/logout');
    }

}
