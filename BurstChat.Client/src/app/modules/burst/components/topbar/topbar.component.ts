import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

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
export class TopbarComponent implements OnInit {

    /**
     * Creates a new instance of TopbarComponent.
     * @memberof TopbarComponent
     */
    constructor(private router: Router) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof TopbarComponent
     */
    public ngOnInit(): void { }

    /*
     * Handles the logout button click event.
     * @memberof TopbarComponent
     */
    public onLogout(): void {
        this.router.navigateByUrl('/session/logout');
    }

}
