import { Component, OnInit, Input } from '@angular/core';
import { faAngleRight, faAngleDown } from '@fortawesome/free-solid-svg-icons';

/**
 * This class represents an angular component that displays an expandable area.
 * @class ExpanderComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-expander',
    templateUrl: './expander.component.html',
    styleUrls: ['./expander.component.scss']
})
export class ExpanderComponent implements OnInit {

    @Input()
    public title: string;

    @Input()
    public expanded = true;

    @Input()
    public loading = false;

    public get icon() {
        return this.expanded ? faAngleDown : faAngleRight;
    }

    /**
     * Creates a new instance of ExpanderComponent.
     * @memberof ExpanderComponent
     */
    constructor() { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ExpanderComponent
     */
    public ngOnInit() { }


    /**
     * Toggles the state of the expander.
     * @memberof ExpanderComponent
     */
    public toggle() {
        this.expanded = !this.expanded;
    }

}
