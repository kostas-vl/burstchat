import { Component, OnInit } from '@angular/core';

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
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof TopbarComponent
     */
    public ngOnInit() { }

}
