import { Component, OnInit } from '@angular/core';

/**
 * This class represents an angular component that displays on screen all components of
 * the burst chat client.
 * @class LayoutComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {

    /**
     * Creates an instance of LayoutComponent.
     * @memberof LayoutComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof LayoutComponent
     */
    public ngOnInit() { }

}
