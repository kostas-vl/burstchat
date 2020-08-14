import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Server } from 'src/app/models/servers/server';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

/**
 * This class represents an angular component that display to the user
 * information about a server.
 * @class EditServerInfoComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-edit-server-info',
    templateUrl: './edit-server-info.component.html',
    styleUrls: ['./edit-server-info.component.scss']
})
export class EditServerInfoComponent implements OnInit {

    @Input()
    public server?: Server;

    public changeAvatarVisible = false;

    public newAvatar?: string;

    /**
     * Creates a new instance of EditServerInfoComponent.
     * @memberof EditServerInfoComponent
     */
    constructor(
        private serversService: ServersService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof EditServerInfoComponent
     */
    public ngOnInit() { }

    /**
     * Handles the change picture link click event.
     * @memberof EditServerInfoComponent
     */
    public onChangeAvatar() {
        this.changeAvatarVisible = true;
    }

    /**
     * Handles the image crop event.
     * @param {string} event The event args.
     * @memberof EditServerInfoComponent
     */
    public onImageCropped(event: string) {
        this.newAvatar = event;
    }

    /**
     * Handles the change picture dialog's save button click event.
     * @memberof EditServerInfoComponent
     */
    public onSaveNewAvatar() {
        const server = { ...this.server, avatar: this.newAvatar };
        this.serversService
            .put(server)
            .subscribe(s => this.chatService.updateServerInfo(s.id));
        this.changeAvatarVisible = false;
    }

    /**
     * Handles the change picture dialog's cancel button click event.
     * @memberof EditServerInfoComponent
     */
    public onCancelNewAvatar() {
        this.changeAvatarVisible = false;
    }

}
