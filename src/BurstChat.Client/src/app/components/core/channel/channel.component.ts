import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCommentAlt } from '@fortawesome/free-solid-svg-icons';
import { Channel } from 'src/app/models/servers/channel';

/**
 * This class represents an angular component that displays information about a single channel
 * and provides action and the ability to connect to its chat.
 * @class ChannelComponent
 */
@Component({
    selector: 'burst-channel',
    templateUrl: './channel.component.html',
    styleUrl: './channel.component.scss',
    standalone: true,
    imports: [FontAwesomeModule]
})
export class ChannelComponent implements OnInit {

    public comment = faCommentAlt;

    @Input()
    public channel?: Channel;

    @Input()
    public isActive = false;

    /**
     * Creates a new instance of ChannelComponent.
     * @memberof ChannelComponent
     */
    constructor(private router: Router) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ChannelComponent
     */
    public ngOnInit() { }

    /**
     * Handles the channel container click event.
     * @memberofChannelComponent
     */
    public onSelect() {
        if (this.channel) {
            this.router.navigate(['/core/chat/channel'], {
                queryParams: {
                    id: this.channel.id,
                    name: this.channel.name
                }
            });
        }
    }

}
