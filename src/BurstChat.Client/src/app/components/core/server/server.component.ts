import { Component, OnInit, Input, OnDestroy, effect } from '@angular/core';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { SidebarService } from 'src/app/services/sidebar/sidebar.service';
import { ServersService } from 'src/app/services/servers/servers.service';
import { ChatService } from 'src/app/services/chat/chat.service';
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
    styleUrl: './server.component.scss',
    standalone: true
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
    ) {
        effect(() => {
            if (this.chatService.onReconnected()) {
                this.chatService.addToServer(this.server.id);
            }
        });
        effect(() => {
            const server = this.serversService.serverInfo();
            if (server?.id === this.server.id) {
                this.server = server;
            }
        })
    }

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
