import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Server } from 'src/app/models/servers/server';
import { SidebarService } from 'src/app/modules/burst/services/sidebar/sidebar.service';
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

    private displaySub?: Subscription;

    private onReconnectedSub?: Subscription;

    public isActive = false;

    @Input()
    public server?: Server;

    public get serverInitials(): string {
        if (this.server && !this.server.avatar) {
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
        private chatService: ChatService
    ) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof ServerComponent
     */
    public ngOnInit() {
        this.displaySub = this
            .sidebarService
            .display
            .subscribe(options => {
                if (options instanceof DisplayServer) {
                    const server = (options as DisplayServer).server;
                    this.isActive = server && server.id === this.server.id;
                    if (this.isActive) {
                        this.server = server;
                    }
                } else {
                    this.isActive = false;
                }
            });

        this.onReconnectedSub = this
            .chatService
            .onReconnected
            .subscribe(() => {
                this.chatService.addToServer(this.server.id);
            });
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof ServerComponent
     */
    public ngOnDestroy() {
        if (this.displaySub) {
            this.displaySub.unsubscribe();
        }

        if (this.onReconnectedSub) {
            this.onReconnectedSub.unsubscribe();
        }
    }

    /**
     * Handles the server button click event.
     * @memberof ServerComponent
     */
    public onSelect() {
        if (this.server && !this.isActive) {
            const options = new DisplayServer(this.server);
            this.sidebarService.toggleDisplay(options);
            this.chatService.addToServer(this.server.id);
        }
    }

}
