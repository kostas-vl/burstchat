import { Component, OnInit } from '@angular/core';
import { Notification } from 'src/app/models/notify/notification';
import { tryParseError } from 'src/app/models/errors/error';
import { Server, BurstChatServer } from 'src/app/models/servers/server';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ServersService } from 'src/app/modules/servers/services/servers/servers.service';

/**
 * This class represents an angular component that displays a form for creating a new BurstChat server.
 * @class AddServerComponent
 */
@Component({
  selector: 'app-add-server',
  templateUrl: './add-server.component.html',
  styleUrls: ['./add-server.component.scss']
})
export class AddServerComponent implements OnInit {

    public server: Server = new BurstChatServer();

    public loading = false;

    /**
     * Creates a new instance of AddServerComponent.
     * @memberof AddServerComponent
     */
    constructor(
        private notifyService: NotifyService,
        private serversService: ServersService
    ) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof AddServerComponent
     */
    public ngOnInit() { }

    /**
     * Handles the create button click event.
     * @memberof AddServerComponent
     */
    public onPost() {

        if (!this.server.name) {
            const notification: Notification = {
                title: 'An error occured',
                content: 'Please provide a name for the new server'
            };
            this.notifyService
                .notify(notification);
            return;
        }

        this.loading = true;

        this.serversService
            .postServer(this.server)
            .subscribe(
                () => {
                    const notification: Notification = {
                        title: 'Success',
                        content: `The server ${this.server.name} was created successfully`
                    };
                    this.notifyService
                        .notify(notification);
                    this.server = new BurstChatServer();
                    this.loading = false;
                },
                httpError => {
                  const error = tryParseError(httpError.error);
                  const content = error
                      ? error.message
                      : '';
                  const notification: Notification = {
                      title: 'An error occured',
                      content: content
                  };
                  this.notifyService
                      .notify(notification);
                  this.loading = false;
                }
            );
    }

}
