import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

/**
 * This class represents an angular component that displays on screen the top bar of the application
 * that provides various types of functionality and actions.
 * @export
 * @class ChatInfoComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-chat-info',
  templateUrl: './chat-info.component.html',
  styleUrls: ['./chat-info.component.scss']
})
export class ChatInfoComponent implements OnInit {

    /**
     * Creates a new instance of ChatInfoComponent.
     * @memberof ChatInfoComponent
     */
    constructor(private router: Router) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChatInfoComponent
     */
    public ngOnInit(): void { }



}