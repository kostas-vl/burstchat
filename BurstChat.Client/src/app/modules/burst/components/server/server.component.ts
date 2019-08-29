import { Component, OnInit, Input } from '@angular/core';
import { Server } from 'src/app/models/servers/server';

/**
 * This class represents an angular component that displays on screen a subscribed server.
 * @export
 * @class ServerComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-server',
    templateUrl: './server.component.html',
    styleUrls: ['./server.component.scss']
})
export class ServerComponent implements OnInit {

    @Input()
    public server?: Server;

    /**
     * Creates an instance of ServerComponent.
     * @memberof ServerComponent
     */
    constructor() { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ServerComponent
     */
    public ngOnInit() { }

}
