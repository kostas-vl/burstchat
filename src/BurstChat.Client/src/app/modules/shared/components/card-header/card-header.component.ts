import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that displays a card header for a card component.
 * @class CardHeaderComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-card-header',
    templateUrl: './card-header.component.html',
    styleUrls: ['./card-header.component.scss']
})
export class CardHeaderComponent implements OnInit {

    /**
     * Creates an instance of CardHeaderComponent.
     * @memberof CardHeaderComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof CardHeaderComponent
     */
    public ngOnInit(): void { }

}
