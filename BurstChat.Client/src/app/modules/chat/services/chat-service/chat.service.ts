import { Injectable } from '@angular/core';
import { HubConnectionBuilder, HubConnection, LogLevel } from '@aspnet/signalr';
import { Subject } from 'rxjs';
import { BurstChatError, tryParseError } from 'src/app/models/errors/error';
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

    private allPrivateGroupMessagesReceivedSource = new Subject<IMessage[]>();

    private privateGroupMessageReceivedSource = new Subject<IMessage>();

    private privateGroupMessageEditedSource = new Subject<IMessage>();

    private privateGroupMessageDeletedSource = new Subject<IMessage>();

    private allChannelMessagesReceivedSource = new Subject<IMessage[]>();

    private channelMessageReceivedSource = new Subject<IMessage>();

    private channelMessageEditedSource = new Subject<IMessage>();

    private channelMessageDeletedSource = new Subject<IMessage>();

    private errorSource = new Subject<BurstChatError>();

    public onAllPrivateGroupMessagesReceived = this.allPrivateGroupMessagesReceivedSource.asObservable();

    public onPrivateGroupMessageReceived = this.privateGroupMessageReceivedSource.asObservable();

    public onPrivateGroupMessageEdited = this.privateGroupMessageEditedSource.asObservable();

    public onPrivateGroupMessageDeleted = this.privateGroupMessageDeletedSource.asObservable();

    public onAllChannelMessagesReceived = this.allChannelMessagesReceivedSource.asObservable();

    public onChannelMessageReceived = this.channelMessageReceivedSource.asObservable();

    public onChannelMessageEdited = this.channelMessageEditedSource.asObservable();

    public onChannelMessageDeleted = this.channelMessageDeletedSource.asObservable();

    public onErrorObservable = this.errorSource.asObservable();

    /**
     * Creates an instance of ChatService.
     * @memberof ChatService
     */
    constructor() { }

    /**
     * This method will process the data sent by a server signal and if the data is not of instance
     * BurstChatError then the next method will be called from the provided subject.
     * @typeparam T The type of data sent by the server.
     * @param {T | BurstChatError} data The data sent by the server.
     * @param Subject<T> source The subject to be executed.
     */
    private ProcessSignal<T>(data: T | BurstChatError, source: Subject<T>): void {
        const error = tryParseError(data);
        if (!error) {
            source.next(data as T);
        }
    }

    /**
     * Establishes a new connection to the chat hub and registers all required callbacks.
     * @memberof ChatService
     */
    public InitializeConnection(): void {
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

        this.connection
            .on('allPrivateGroupMessagesReceived', data => this.ProcessSignal(data, this.allPrivateGroupMessagesReceivedSource));

        this.connection
            .on('privateGroupMessageReceived', data => this.ProcessSignal(data, this.privateGroupMessageReceivedSource));

        this.connection
            .on('privateGroupMessageEdited', data => this.ProcessSignal(data, this.privateGroupMessageEditedSource));

        this.connection
            .on('privateGroupMessageDeleted', data => this.ProcessSignal(data, this.privateGroupMessageDeletedSource));

        this.connection
            .on('allChannelMessagesReceived', data => this.ProcessSignal(data, this.allChannelMessagesReceivedSource));

        this.connection
            .on('channelMessageReceived', data => this.ProcessSignal(data, this.channelMessageReceivedSource));

        this.connection
            .on('channelMessageEdited', data => this.ProcessSignal(data, this.channelMessageEditedSource));

        this.connection
            .on('channelMessageDeleted', data => this.ProcessSignal(data, this.channelMessageDeletedSource));

        this.connection
            .start()
            .catch(error => console.log(error));
    }

    /**
     * Closes the active connection and executes any necessary clean up code.
     * @memberof ChatService
     */
    public DisposeConnection(): void {
        if (this.connection) {
            this.connection.stop();
        }
    }

    /**
     * Signals for all messages of a private group to be received.
     * @param {number} groupId The id of the group.
     */
    public getAllPrivateGroupMessages(groupId: number) {
        if (this.connection) {
            this.connection
                .invoke('getAllPrivateGroupMessages', groupId)
                .catch(error => console.log(error));
        }
    }
    /**
     * Sends a new chat message to the server in order to be trasmitted to all users of a private group.
     * @param {IMessage} message The message to be sent.
     * @memberof ChatService
     */
    public postPrivateGroupMessage(message: IMessage): void {
        if (this.connection) {
            this.connection
                .invoke('postPrivateGroupMessage', message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a new edit to an existing message in a private group.
     * @param {number} groupId The id of the private group.
     * @param {IMessage} message The edited message.
     */
    public editPrivateGroupMessage(groupId: number, message: IMessage): void {
        if (this.connection) {
            this.connection
                .invoke('putPrivateGroupMessage', groupId, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a delete on an existing message of a private group.
     * @param {number} groupId The id of the private group.
     * @param {IMessage} message The message to be deleted.
     */
    public deletePrivateGroupMessage(groupId: number, message: IMessage): void {
        if (this.connection) {
            this.connection
                .invoke('deletePrivateGroupMessage', groupId, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for all the messages of a channel to be received.
     * @param {number} channelId The id of the channel.
     */
    public getAllChannelMessages(channelId: number): void {
        if (this.connection) {
            this.connection
                .invoke('getAllChannelMessages', channelId)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a new message to be posted to a channel.
     * @param {number} channelId The id of the channel.
     * @param {IMessage} message The message to be posted.
     */
    public postChannelMessage(channelId: number, message: IMessage): void {
        if (this.connection) {
            this.connection
                .invoke('postChannelMessage', channelId, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a new edit on an existing message of a channel.
     * @param {number} channelId The id of the channel.
     * @param {IMessage} message The edited message.
     */
    public putChannelMessage(channelId: number, message: IMessage): void {
        if (this.connection) {
            this.connection
                .invoke('putChannelMessage', channelId, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a delete of a message on a channel.
     * @param {number} channelId The id of the channel.
     * @param {IMessage} message The message to be deleted.
     */
    public deleteChannelMessage(channelId: number, message: IMessage): void {
        if (this.connection) {
            this.connection
                .invoke('deleteChannelMessage', channelId, message)
                .catch(error => console.log(error));
        }
    }

}
