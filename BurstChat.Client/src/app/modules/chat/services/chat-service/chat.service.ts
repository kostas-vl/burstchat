import { Injectable } from '@angular/core';
import { HubConnectionBuilder, HubConnection, LogLevel } from '@aspnet/signalr';
import { Subject } from 'rxjs';
import { IMessage } from 'src/app/models/chat/message';
import { environment } from 'src/environments/environment';

/**
 * This class represents an angular service that connects to the remote signalr server and trasmits messages related to
 * the chat.
 * @export
 * @class ChatService
 */
@Injectable()
export class ChatService {

    private connection?: HubConnection;

    private onMessageReceivedSource = new Subject<IMessage>();

    public onMessageReceivedObservable = this.onMessageReceivedSource.asObservable();

    /**
     * Creates an instance of ChatService.
     * @memberof ChatService
     */
    constructor() { }

    /**
     * Establishes a new connection to the chat hub and registers all required callbacks.
     * @memberof ChatService
     */
    public InitializeConnection() {
        try {
            const builder = new HubConnectionBuilder();
            builder.withUrl('/chat');

            if (!environment.production) {
                builder.configureLogging(LogLevel.Information);
            }

            this.connection = builder.build();
        } catch {
            this.connection = undefined;
            return;
        }

        this.connection.on('messageReceived', _ => this.onMessageReceivedSource.next(_));

        this.connection
            .start()
            .catch(error => console.log(error));
    }

    /**
     * Closes the active connection and executes any necessary clean up code.
     * @memberof ChatService
     */
    public DisposeConnection() {
        if (this.connection) {
            this.connection.stop();
        }
    }

    /**
     * Sends a new chat message to the server in order to be trasmitted to all other users.
     * @param {IMessage} message The message to be sent.
     * @memberof ChatService
     */
    public sendMessage(message: IMessage) {
        if (this.connection) {
            this.connection
                .invoke('sendMessage', message)
                .catch(error => console.log(error));
        }
    }

}
