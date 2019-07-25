import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that diplays a card to the user.
 * @class CardComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss']
})
export class CardComponent implements OnInit {

    /**
     * Creates an instance of CardComponent.
     * @memberof CardComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof CardComponent
     */
    public ngOnInit() { }

}
