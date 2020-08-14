import { Component, OnInit } from '@angular/core';
import { faDragon } from '@fortawesome/free-solid-svg-icons';

/**
 * This class represents an angular component that displays a home screen to the user.
 * @export
 * @class HomeComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

    public dragon = faDragon;

    /**
     * Creates an instance of HomeComponent.
     * @memberof HomeComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof HomeComponent
     */
    public ngOnInit() { }

}
