import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that displays the main content of a card component.
 * @class CardBodyComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-card-body',
  templateUrl: './card-body.component.html',
  styleUrls: ['./card-body.component.scss']
})
export class CardBodyComponent implements OnInit {

    /**
     * Creates an instane of CardBodyComponent.
     * @memberof CardBodyComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof CardBodyComponent
     */
    public ngOnInit(): void { }

}
