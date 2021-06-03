import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { SidebarService } from 'src/app/modules/burst/services/sidebar/sidebar.service';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';
import { DisplayServer } from 'src/app/models/sidebar/display-server';

/**
 * This class represents an angular component that displays on screen a subscribed server.
 * @export
 * @class ServerComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-server',
    templateUrl: './server.component.html',
    styleUrls: ['./server.component.scss']
})
export class ServerComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[] = [];

    public isActive = false;

    @Input()
    public server?: Server;

    public get avatarExists() {
        return this.server.avatar?.length > 0 && this.server.avatar !== '\\x';
    }

    public get serverInitials(): string {
        if (this.server) {
            const words = this.server.name.split(' ');
            return words.reduce((acc, n) => acc + n[0]?.toUpperCase(), '');
        }

        return '';
    }

    /**
     * Creates an instance of ServerComponent.
     * @memberof ServerComponent
     */
    constructor(
        private sidebarService: SidebarService,
        private serversService: ServersService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ServerComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.sidebarService
                .display
                .subscribe(options => {
                    if (options instanceof DisplayServer) {
                        this.isActive = options.serverId === this.server.id;
                    } else {
                        this.isActive = false;
                    }
                }),
            this.serversService
                .serverInfo
                .subscribe(server => {
                    if (server?.id === this.server.id) {
                        this.server = server;
                    }
                }),
            this.chatService
                .onReconnected$
                .subscribe(() => this.chatService.addToServer(this.server.id))
        ];

    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ServerComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }

    /**
     * Handles the server button click event.
     * @memberof ServerComponent
     */
    public onSelect() {
        if (this.server && !this.isActive) {
            const options = new DisplayServer(this.server.id);
            this.sidebarService.toggleDisplay(options);
            this.chatService.addToServer(this.server.id);
        }
    }

}
