import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { ServersService } from 'src/app/modules/burst/services/servers/servers.service';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

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
export class ServerComponent implements OnInit, OnDestroy {

    private onReconnectedSubscription?: Subscription;

    private activeServerSubscription?: Subscription;

    public isActiveServer = false;

    @Input()
    public server?: Server;

    public get serverInitials(): string {
        if (this.server) {
            return this
                .server
                .name
                .slice(0, 1)
                .toUpperCase();
        }

        return '';
    }

    /**
     * Creates an instance of ServerComponent.
     * @memberof ServerComponent
     */
    constructor(
        private serversService: ServersService,
        private chatService: ChatService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ServerComponent
     */
    public ngOnInit() {
        this.onReconnectedSubscription = this
            .chatService
            .onReconnected
            .subscribe(() => {
                this.chatService.addToServer(this.server.id);
            });

        this.activeServerSubscription = this
            .serversService
            .activeServer
            .subscribe(server => {
                this.isActiveServer = server && server.id === this.server.id;
                if (this.isActiveServer) {
                    this.server = server;
                }
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ServerComponent
     */
    public ngOnDestroy() {
        if (this.onReconnectedSubscription) {
            this.onReconnectedSubscription
                .unsubscribe();
        }

        if (this.activeServerSubscription) {
            this.activeServerSubscription
                .unsubscribe();
        }
    }

    /**
     * Handles the server button click event.
     * @memberof ServerComponent
     */
    public onSelect() {
        if (this.server) {
            this.serversService
                .setActiveServer(this.server);

            this.chatService
                .addToServer(this.server.id);
        }
    }

}
