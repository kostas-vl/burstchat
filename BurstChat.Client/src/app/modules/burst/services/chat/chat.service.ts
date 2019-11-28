import { Injectable } from '@angular/core';
import { HubConnectionBuilder, HubConnection, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { Subject, BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { BurstChatError, tryParseError } from 'src/app/models/errors/error';
import { Payload } from 'src/app/models/signal/payload';
import { Server } from 'src/app/models/servers/server';
import { Message } from 'src/app/models/chat/message';
import { Invitation } from 'src/app/models/servers/invitation';
import { StorageService } from 'src/app/services/storage/storage.service';
import { NotifyService } from 'src/app/services/notify/notify.service';
import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { PrivateGroupConnectionOptions } from 'src/app/models/chat/private-group-connection-options';
import { ChannelConnectionOptions } from 'src/app/models/chat/channel-connection-options';
import { Channel } from 'src/app/models/servers/channel';
import { Subscription } from 'src/app/models/servers/subscription';

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

    private onReconnectedSource = new Subject();

    private addedServerSource = new Subject<Server>();

    private subscriptionDeletedSource = new Subject<[number, Subscription]>();

    private channelCreatedSource = new Subject<[number, Channel]>();

    private channelUpdatedSource = new Subject<Channel>();

    private channelDeletedSource = new Subject<number>();

    private invitationsSource = new BehaviorSubject<Invitation[]>([]);

    private newInvitationSource = new Subject<Invitation>();

    private updatedInvitationSource = new Subject<Invitation>();

    private selfAddedToChatSource = new Subject();

    private allMessagesReceivedSource = new Subject<Payload<Message[]>>();

    private messageReceivedSource = new Subject<Payload<Message>>();

    private messageEditedSource = new Subject<Payload<Message>>();

    private messageDeletedSource = new Subject<Payload<Message>>();

    private newDirectMessagingSource = new Subject<Payload<any>>();

    private errorSource = new Subject<BurstChatError>();

    public onConnected = this.onConnectedSource.asObservable();

    public onReconnected = this.onReconnectedSource.asObservable();

    public addedServer = this.addedServerSource.asObservable();

    public subscriptionDeleted = this.subscriptionDeletedSource.asObservable();

    public channelCreated = this.channelCreatedSource.asObservable();

    public channelUpdated = this.channelUpdatedSource.asObservable();

    public channelDeleted = this.channelDeletedSource.asObservable();

    public invitations = this.invitationsSource.asObservable();

    public newInvitation = this.newInvitationSource.asObservable();

    public updatedInvitation = this.updatedInvitationSource.asObservable();

    public selfAddedToChat = this.selfAddedToChatSource.asObservable();

    public allMessagesReceived = this.allMessagesReceivedSource.asObservable();

    public messageReceived = this.messageReceivedSource.asObservable();

    public messageEdited = this.messageEditedSource.asObservable();

    public messageDeleted = this.messageDeletedSource.asObservable();

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
            builder
                .withUrl('/chat', {
                    accessTokenFactory: () => this.storageService.tokenInfo.accessToken
                })
                .withAutomaticReconnect();

            if (!environment.production) {
                builder.configureLogging(LogLevel.Information);
            }

            this.connection = builder.build();
        } catch {
            this.connection = undefined;
            return;
        }

        this.connection.onreconnected(connectionId => this.onReconnectedSource.next());

        this.connection.on('addedServer', data => this.ProcessRawSignal(data, this.addedServerSource));

        this.connection.on('subscriptionDeleted', data => this.ProcessRawSignal(data, this.subscriptionDeletedSource));

        this.connection.on('channelCreated', data => this.ProcessRawSignal(data, this.channelCreatedSource));

        this.connection.on('channelUpdated', data => this.ProcessRawSignal(data, this.channelUpdatedSource));

        this.connection.on('channelDeleted', data => this.ProcessRawSignal(data, this.channelDeletedSource));

        this.connection.on('invitations', data => this.ProcessRawSignal(data, this.invitationsSource));

        this.connection.on('newInvitation', data => this.ProcessRawSignal(data, this.newInvitationSource));

        this.connection.on('updatedInvitation', data => this.ProcessRawSignal(data, this.updatedInvitationSource));

        this.connection.on('selfAddedToPrivateGroup', () => {
            setTimeout(() => this.selfAddedToChatSource.next(), 500);
        });

        this.connection.on('allPrivateGroupMessagesReceived', data => this.ProcessSignal(data, this.allMessagesReceivedSource));

        this.connection.on('privateGroupMessageReceived', data => this.ProcessSignal(data, this.messageReceivedSource));

        this.connection.on('privateGroupMessageEdited', data => this.ProcessSignal(data, this.messageEditedSource));

        this.connection.on('privateGroupMessageDeleted', data => this.ProcessSignal(data, this.messageDeletedSource));

        this.connection.on('selfAddedToChannel', () => {
            setTimeout(() => this.selfAddedToChatSource.next(), 500);
        });

        this.connection.on('allChannelMessagesReceived', data => this.ProcessSignal(data, this.allMessagesReceivedSource));

        this.connection.on('channelMessageReceived', data => this.ProcessSignal(data, this.messageReceivedSource));

        this.connection.on('channelMessageEdited', data => this.ProcessSignal(data, this.messageEditedSource));

        this.connection.on('channelMessageDeleted', data => this.ProcessSignal(data, this.messageDeletedSource));

        this.connection.on('selfAddedToDirectMessaging', () => {
            setTimeout(() => this.selfAddedToChatSource.next(), 500);
        });

        this.connection.on('newDirectMessaging', data => {
            this.ProcessSignal(data, this.newDirectMessagingSource);
            setTimeout(() => this.selfAddedToChatSource.next(), 500);
        });

        this.connection.on('allDirectMessagesReceived', data => this.ProcessSignal(data, this.allMessagesReceivedSource));

        this.connection.on('directMessageReceived', data => this.ProcessSignal(data, this.messageReceivedSource));

        this.connection.on('channelMessageEdited', data => this.ProcessSignal(data, this.messageEditedSource));

        this.connection.on('channelMessageDeleted', data => this.ProcessSignal(data, this.messageDeletedSource));

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
     * Creates a new BurstChat server based on the provided instance.
     * @param {Server} server The server instance to be created.
     * @memberof ChatService
     */
    public addServer(server: Server) {
        if (this.connection) {
            this.connection
                .invoke('addServer', server)
                .catch(error => console.log(error));
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
     * Removes a subscription from a BurstChat Server.
     * @param {number} serverId The id of the server.
     * @param {Subscription} subscription The subscription to be removed.
     * @memberof ChatService
     */
    public deleteSubscription(serverId: number, subscription: Subscription) {
        if (this.connection) {
            this.connection
                .invoke('deleteSubscription', serverId, subscription)
                .catch(error => console.log(error));
        }
    }

    /**
     * Creates a new channel to an existing server.
     * @param {number} serverId The id of the server,
     * @param {Channel} channel The channel information
     * @memberof ChatService
     */
    public postChannel(serverId: number, channel: Channel) {
        if (this.connection) {
            this.connection
                .invoke('postChannel', serverId, channel)
                .catch(err => console.log(err));
        }
    }

    /**
     * Updates the details of an existing channel.
     * @param {number} serverId The id of the channel's server.
     * @param {Channel} channel The updated channel information.
     * @memberof ChatService
     */
    public putChannel(serverId: number, channel: Channel) {
        if (this.connection) {
            this.connection
                .invoke('putChannel', serverId, channel)
                .catch(err => console.log(err));
        }
    }

    /**
     * Deletes a channel from a server.
     * @param {number} serverId The id of the channel's server.
     * @param {number} channelId The id of the channel.
     * @memberof ChatService
     */
    public deleteChannel(serverId: number, channelId: number) {
        if (this.connection) {
            this.connection
                .invoke('deleteChannel', serverId, channelId)
                .catch(err => console.log(err));
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
     * @param {number} serverId The id of the server the invitation will be sent from.
     * @param {string} username The name of the user the invitation will be sent to.
     * @memberof ChatService
     */
    public sendInvitation(serverId: number, username: string) {
        if (this.connection) {
            this.connection
                .invoke('sendInvitation', serverId, username)
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
     * Adds the current established connection to a chat defined by the provided options.
     * @memberof ChatService
     * @param {ChatConnectionOptions} options The options to be used for the proper call to the signal server.
     */
    public addSelfToChat(options: ChatConnectionOptions) {
        if (this.connection && options) {
            let methodName = '';

            if (options instanceof PrivateGroupConnectionOptions) {
                methodName = 'addToPrivateGroupConnection';
            } else if (options instanceof ChannelConnectionOptions) {
                methodName = 'addToChannelConnection';
            } else {
                methodName = 'addToDirectMessaging';
            }

            this.connection
                .invoke(methodName, options.id)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for all messages of a chat to be received.
     * @param {ChatConnectionOptions} options The options to be used for the proper call to the signal server.
     */
    public getAllMessages(options: ChatConnectionOptions, lastMessageId?: number) {
        if (this.connection && options) {
            let methodName = '';
            let args = [];

            if (options instanceof PrivateGroupConnectionOptions) {
                methodName = 'getAllPrivateGroupMessages';
                args = [options.id];
            } else if (options instanceof ChannelConnectionOptions) {
                methodName = 'getAllChannelMessages';
                args = [options.id];
            } else {
                methodName = 'getAllDirectMessages';
                args = [options.id, lastMessageId];
            }

            this.connection
                .invoke(methodName, ...args)
                .catch(error => console.log(error));
        }
    }
    /**
     * Sends a new message to the server in order to be trasmitted to all users of a chat.
     * @param {ChatConnectionOptions} options The options to be used for the proper call to the signal server.
     * @param {Message} message The message to be sent.
     * @memberof ChatService
     */
    public postMessage(options: ChatConnectionOptions, message: Message): void {
        if (this.connection && options) {
            let methodName = '';

            if (options instanceof PrivateGroupConnectionOptions) {
                methodName = 'postPrivateGroupMessage';
            } else if (options instanceof ChannelConnectionOptions) {
                methodName = 'postChannelMessage';
            } else {
                methodName = 'postDirectMessage';
            }

            this.connection
                .invoke(methodName, options.id, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a new edit to an existing message in a chat.
     * @param {ChatConnectionOptions} options The options to be used for the proper call to the signal server.
     * @param {Message} message The edited message.
     */
    public editMessage(options: ChatConnectionOptions, message: Message): void {
        if (this.connection && options) {
            let methodName = '';

            if (options instanceof PrivateGroupConnectionOptions) {
                methodName = 'putPrivateGroupMessage';
            } else if (options instanceof ChannelConnectionOptions) {
                methodName = 'putChannelMessage';
            } else {
                methodName = 'putDirectMessage';
            }

            this.connection
                .invoke(methodName, options.id, message)
                .catch(error => console.log(error));
        }
    }

    /**
     * Signals for a delete on an existing message of a chat.
     * @param {ChatConnectionOptions} options The options to be used for the proper call to the signal server.
     * @param {Message} message The message to be deleted.
     */
    public deleteMessage(options: ChatConnectionOptions, message: Message): void {
        if (this.connection && options) {
            let methodName = '';

            if (options instanceof PrivateGroupConnectionOptions) {
                methodName = 'deletePrivateGroupMessage';
            } else if (options instanceof ChannelConnectionOptions) {
                methodName = 'deleteChannelMessage';
            } else {
                methodName = 'deleteDirectMessage';
            }

            this.connection
                .invoke(methodName, options.id, message)
                .catch(error => console.log(error));
        }
    }

}
