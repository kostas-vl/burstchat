import { Component, OnInit, Input } from '@angular/core';

/**
 * This class represents an angular component that displays a shimmer loader for a list of elements.
 * @export
 * @class ShimmerListComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-shimmer-list',
    templateUrl: './shimmer-list.component.html',
    styleUrl: './shimmer-list.component.scss',
    standalone: true,
})
export class ShimmerListComponent implements OnInit {

    @Input()
    public showIcon = true;

    @Input()
    public showSmallLines = true;

    /**
     * Creates an instance of ShimmerListComponent.
     * @memberof ShimmerListComponent
     */
    constructor() { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ShimmerListComponent
     */
    public ngOnInit() { }

}
