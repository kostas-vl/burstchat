import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that displays the footer in a card component.
 * @class CardFooterComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-card-footer',
    templateUrl: './card-footer.component.html',
    styleUrls: ['./card-footer.component.scss']
})
export class CardFooterComponent implements OnInit {

    /**
     * Creates an instance of CardFooterComponent.
     * @memberof CardFooterComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof CardFooterComponent
     */
    public ngOnInit(): void { }

}
