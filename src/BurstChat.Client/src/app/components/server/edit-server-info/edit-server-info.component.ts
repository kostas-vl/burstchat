import { Component, OnInit, Input, Signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Server } from 'src/app/models/servers/server';
import { ServersService } from 'src/app/services/servers/servers.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { ImageCropComponent } from 'src/app/components/shared/image-crop/image-crop.component';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';
import { AvatarComponent } from 'src/app/components/shared/avatar/avatar.component';

/**
 * This class represents an angular component that display to the user
 * information about a server.
 * @class EditServerInfoComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-edit-server-info',
    templateUrl: './edit-server-info.component.html',
    styleUrl: './edit-server-info.component.scss',
    standalone: true,
    imports: [
        DatePipe,
        DialogComponent,
        ImageCropComponent,
        AvatarComponent,
        CardComponent,
        CardBodyComponent,
        CardFooterComponent
    ]
})
export class EditServerInfoComponent implements OnInit {

    @Input({ required: true })
    public server: Signal<Server | null>;

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
        const server = { ...this.server(), avatar: this.newAvatar };
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
