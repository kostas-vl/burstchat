import { Component, OnInit, Input } from '@angular/core';
import { Channel } from 'src/app/models/servers/channel';

/**
 * This class represents an angular component that displays information about a single channel
 * and provides action and the ability to connect to its chat.
 * @class ChannelComponent
 */
@Component({
  selector: 'app-channel',
  templateUrl: './channel.component.html',
  styleUrls: ['./channel.component.scss']
})
export class ChannelComponent implements OnInit {

    @Input()
    public channel?: Channel;

    /**
     * Creates a new instance of ChannelComponent.
     * @memberof ChannelComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChannelComponent
     */
    public ngOnInit() { }

}
