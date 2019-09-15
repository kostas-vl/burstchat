import { Injectable } from '@angular/core';
import { HubConnectionBuilder, HubConnection, HubConnectionState, LogLevel } from '@aspnet/signalr';
import { Subject, BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { BurstChatError, tryParseError } from 'src/app/models/errors/error';
import { Payload } from 'src/app/models/signal/payload';
import { Message } from 'src/app/models/chat/message';
import { Invitation } from 'src/app/models/servers/invitation';
import { StorageService } from 'src/app/services/storage/storage.service';
import { NotifyService } from 'src/app/services/notify/notify.service';

/**
 * This class represents an angular service that connects to the remote signalr server and trasmits messages related to
 * the chat.
 * @export
 * @class ChatService
 */
@Injectable()
export class ChatService {

    private connection?: HubConnection;

    private onConnectedSource = new Subject();

    private invitationsSource = new BehaviorSubject<Invitation[]>([]);

    private newInvitationSource = new Subject<Invitation>();

    private updatedInvitationSource = new Subject<Invitation>();

    private selfAddedToPrivateGroupSource = new Subject();

    private allPrivateGroupMessagesReceivedSource = new Subject<Payload<Message[]>>();

    private privateGroupMessageReceivedSource = new Subject<Payload<Message>>();

    private privateGroupMessageEditedSource = new Subject<Payload<Message>>();

    private privateGroupMessageDeletedSource = new Subject<Payload<Message>>();

    private selfAddedToChannelSource = new Subject();

    private allChannelMessagesReceivedSource = new Subject<Payload<Message[]>>();

    private channelMessageReceivedSource = new Subject<Payload<Message>>();

    private channelMessageEditedSource = new Subject<Payload<Message>>();

    private channelMessageDeletedSource = new Subject<Payload<Message>>();

    private errorSource = new Subject<BurstChatError>();

    public onConnected = this.onConnectedSource.asObservable();

    public invitations = this.invitationsSource.asObservable();

    public newInvitation = this.newInvitationSource.asObservable();

    public updatedInvitation = this.updatedInvitationSource.asObservable();

    public onSelfAddedToPrivateGroup = this.selfAddedToPrivateGroupSource.asObservable();

    public onAllPrivateGroupMessagesReceived = this.allPrivateGroupMessagesReceivedSource.asObservable();

    public onPrivateGroupMessageReceived = this.privateGroupMessageReceivedSource.asObservable();

    public onPrivateGroupMessageEdited = this.privateGroupMessageEditedSource.asObservable();

    public onPrivateGroupMessageDeleted = this.privateGroupMessageDeletedSource.asObservable();

    public onSelfAddedToChannel = this.selfAddedToChannelSource.asObservable();

    public onAllChannelMessagesReceived = this.allChannelMessagesReceivedSource.asObservable();

    public onChannelMessageReceived = this.channelMessageReceivedSource.asObservable();

    public onChannelMessageEdited = this.channelMessageEditedSource.asObservable();

    public onChannelMessageDeleted = this.channelMessageDeletedSource.asObservable();

    public onErrorObservable = this.errorSource.asObservable();

    /**
     * Creates an instance of ChatService.
     * @memberof ChatService
     */
    constructor(
        private storageService: StorageService,
        private notifyService: NotifyService
    ) { }

    /**
     * This method will process the data sent by a server signal and if the data is not of instance
     * BurstChatError then the next method will be called from the provided subject.
     * @private
     * @template T The success type contained within the payload.
     * @param {(Payload<T | BurstChatError>)} payload The payload sent fron the signal server.
     * @param {Subject<Payload<T>>} source The subject to be executed.
     * @memberof ChatService
     */
    private ProcessSignal<T>(payload: Payload<T | BurstChatError>, source: Subject<Payload<T>>): void {
        const error = tryParseError(payload.content);
        if (!error) {
            source.next(payload as Payload<T>);
        } else {
            this.notifyService.notifyError(error);
        }
    }

    /**
     * This method will process the data sent by a server signal and if the data is not of instance
     * BurstChatError then the next method will be called from the provided subject.
     * @private
     * @template T The success type contained within the payload.
     * @param {T | BurstChatError} data The data sent fron the signal server.
     * @param {Subject<T>} source The subject to be executed.
     * @memberof ChatService
     */
    private ProcessRawSignal<T>(data: T | BurstChatError, source: Subject<T>): void {
        const error = tryParseError(data);
        if (!error) {
            source.next(data as T);
        } else {
            this.notifyService.notifyError(error);
        }
    }

    /**
     * Establishes a new connection to the chat hub and registers all required callbacks.
     * @memberof ChatService
     */
    public InitializeConnection(): void {
        try {
            const builder = new HubConnectionBuilder();
            builder.withUrl('/chat', {
                accessTokenFactory: () => this.storageService.tokenInfo.accessToken
            });

            if (!environment.production) {
                builder.configureLogging(LogLevel.Information);
            }

            this.connection = builder.build();
        } catch {
            this.connection = undefined;
            return;
        }

        this.connection
            .on('invitations', data => this.ProcessRawSignal(data, this.invitationsSource));

        this.connection
            .on('newInvitation', data => this.ProcessRawSignal(data, this.newInvitationSource));

        this.connection
            .on('updatedInvitation', data => this.ProcessRawSignal(data, this.updatedInvitationSource));

        this.connection
            .on('selfAddedToPrivateGroup', () => this.selfAddedToPrivateGroupSource.next());

        this.connection
            .on('allPrivateGroupMessagesReceived', data => this.ProcessSignal(data, this.allPrivateGroupMessagesReceivedSource));

        this.connection
            .on('privateGroupMessageReceived', data => this.ProcessSignal(data, this.privateGroupMessageReceivedSource));

        this.connection
            .on('privateGroupMessageEdited', data => this.ProcessSignal(data, this.privateGroupMessageEditedSource));

        this.connection
            .on('privateGroupMessageDeleted', data => this.ProcessSignal(data, this.privateGroupMessageDeletedSource));

        this.connection
            .on('selfAddedToChannel', () => this.selfAddedToChannelSource.next());

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
            .then(() => this.onConnectedSource.next())
            .catch(error => console.log(error));
    }

    /**
     * Closes the active connection and executes any necessary clean up code.
     * @memberof ChatService
     */
    public DisposeConnection(): void {
        if (this.connection && this.connection.state === HubConnectionState.Connected) {
            this.connection.stop();
        }
    }

    /**
     * Adds the current user signal connection to a server group defined by the provided server id.
     * @param {number} serverId The id of the target server.
     * @memberof ChatService
     */
    public addToServer(serverId: number) {
        if (this.connection) {
            this.connection
                .invoke('addToServer', serverId)
                .catch(error => console.log(error));
        }
    }

    /**
     * Fetches all invitations sent to the current user by servers.
     * @memberof ChatService
     */
    public getInvitations() {
        if (this.connection) {
            this.connection
                .invoke('getInvitations')
                .catch(error => console.log(error));
        }
    }

    /**
     * Sends a new invitation to a user based on the provided invitation instance.
     * @param {Invitation} invitation The invitation details.
     * @memberof ChatService
     */
    /**
     * Sends a new invitation to a user based on the provided invitation instance.
     * @param {number} serverId The id of the server the invitation will be sent from.
     * @param {number} userId The id of the user the invitation will be sent to.
     * @memberof ChatService
     */
    public sendInvitation(serverId: number, userId: number) {
        if (this.connection) {
            this.connection
                .invoke('sendInvitation', serverId, userId)
                .catch(error => console.log(error));
        }
    }

    /**
     * Updates the invitation sent to user.
     * @param {Invitation} invitation The modified invitation details.
     * @memberof ChatService
     */
    public updateInvitation(invitation: Invitation) {
        if (this.connection) {
            this.connection
                .invoke('updateInvitation', invitation)
                .catch(error => console.log(error));
        }
    }

    /**
     * Adds the current established connection to a group defined by the provided private group id.
     * @memberof ChatService
     * @param {number} groupId The id of the private group.
     */
    public addSelfToPrivateGroup(groupId: number) {
        if (this.connection) {
            this.connection
                .invoke('addToPrivateGroupConnection', groupId)
                .catch(error => console.log(error));
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
     * @param {number} groupId The id of the target group.
     * @param {Message} message The message to be sent.
     * @memberof ChatService
     */
    public postPrivateGroupMessage(groupId: number, message: Message): void {
        if (this.connection) {
            this.connection
                .invoke('postPrivateGroupMessage', message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a new edit to an existing message in a private group.
     * @param {number} groupId The id of the private group.
     * @param {Message} message The edited message.
     */
    public editPrivateGroupMessage(groupId: number, message: Message): void {
        if (this.connection) {
            this.connection
                .invoke('putPrivateGroupMessage', groupId, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a delete on an existing message of a private group.
     * @param {number} groupId The id of the private group.
     * @param {Message} message The message to be deleted.
     */
    public deletePrivateGroupMessage(groupId: number, message: Message): void {
        if (this.connection) {
            this.connection
                .invoke('deletePrivateGroupMessage', groupId, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Adds the current established connection to a group defined by the provided channel id.
     * @memberof ChatService
     * @param {number} channelId The id of the channel.
     */
    public addSelfToChannel(channelId: number) {
        if (this.connection) {
            this.connection
                .invoke('addToChannelConnection', channelId)
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
     * @param {Message} message The message to be posted.
     */
    public postChannelMessage(channelId: number, message: Message): void {
        if (this.connection) {
            this.connection
                .invoke('postChannelMessage', channelId, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a new edit on an existing message of a channel.
     * @param {number} channelId The id of the channel.
     * @param {Message} message The edited message.
     */
    public putChannelMessage(channelId: number, message: Message): void {
        if (this.connection) {
            this.connection
                .invoke('putChannelMessage', channelId, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a delete of a message on a channel.
     * @param {number} channelId The id of the channel.
     * @param {Message} message The message to be deleted.
     */
    public deleteChannelMessage(channelId: number, message: Message): void {
        if (this.connection) {
            this.connection
                .invoke('deleteChannelMessage', channelId, message)
                .catch(error => console.log(error));
        }
    }

}
