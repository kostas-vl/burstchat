import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { Server, BurstChatServer } from 'src/app/models/servers/server';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { SidebarService } from 'src/app/services/sidebar/sidebar.service';
import { DialogComponent } from 'src/app/components/shared/dialog/dialog.component';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardHeaderComponent } from 'src/app/components/shared/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/components/shared/card-footer/card-footer.component';

/**
 * This class represents an angular component that displays a form for creating a new BurstChat server.
 * @class AddServerComponent
 */
@Component({
    selector: 'burst-add-server',
    templateUrl: './add-server.component.html',
    styleUrl: './add-server.component.scss',
    standalone: true,
    imports: [
        FormsModule,
        DialogComponent,
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent
    ]
})
export class AddServerComponent implements OnInit, OnDestroy {

    private subscriptions?: Subscription[];

    public closeIcon = faTimes;

    public server: Server = new BurstChatServer();

    public loading = false;

    public visible = false;

    /**
     * Creates a new instance of AddServerComponent.
     * @memberof AddServerComponent
     */
    constructor(
        private notifyService: NotifyService,
        private chatService: ChatService,
        private sidebarService: SidebarService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof AddServerComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this.chatService
                .addedServer$
                .subscribe(server => {
                    if (server.name === this.server.name) {
                        const title = 'Success';
                        const content = `The server ${this.server.name} was created successfully`;
                        this.notifyService.notify(title, content);
                        this.server = new BurstChatServer();
                        this.loading = false;
                        this.onClose();
                    }
                }),
            this.sidebarService
                .addServerDialog
                .subscribe(visible => this.visible = visible)
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof AddServerComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }

    /**
     * Handles the close button click event.
     * @memberof AddServerComponent
     */
    public onClose() {
        this.sidebarService.toggleAddServerDialog(false);
        this.server = new BurstChatServer();
    }

    /**
     * Handles the create button click event.
     * @memberof AddServerComponent
     */
    public onPost() {
        if (!this.server.name) {
            const title = 'An error occured';
            const content = 'Please provide a name for the new server';
            this.notifyService.notify(title, content);
            return;
        }
        this.loading = true;
        this.chatService.addServer(this.server);
        this.server = new BurstChatServer();
    }

}
