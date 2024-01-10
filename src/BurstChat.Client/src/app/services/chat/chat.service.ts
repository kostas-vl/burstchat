import { effect, Injectable, signal, WritableSignal } from '@angular/core';
import { HubConnectionBuilder, HubConnection, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { BurstChatError, tryParseError } from 'src/app/models/errors/error';
import { TokenInfo } from 'src/app/models/identity/token-info';
import { Payload } from 'src/app/models/signal/payload';
import { Server } from 'src/app/models/servers/server';
import { User } from 'src/app/models/user/user';
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

    private onConnectedSource = signal(false);

    private onReconnectedSource = signal(false);

    private addedServerSource: WritableSignal<Server | null> = signal(null);

    private updatedServerSource: WritableSignal<Server | null> = signal(null);

    private subscriptionDeletedSource: WritableSignal<[number, Subscription] | null> = signal(null);

    private channelCreatedSource: WritableSignal<[number, Channel] | null> = signal(null);

    private channelUpdatedSource: WritableSignal<Channel | null> = signal(null);

    private channelDeletedSource: WritableSignal<number | null> = signal(null);

    private userUpdatedSource: WritableSignal<User | null> = signal(null);

    private invitationsSource: WritableSignal<Invitation[]> = signal([]);

    private newInvitationSource: WritableSignal<Invitation | null> = signal(null);

    private updatedInvitationSource: WritableSignal<Invitation | null> = signal(null);

    private selfAddedToChatSource: WritableSignal<boolean> = signal(false);

    private allMessagesReceivedSource: WritableSignal<Payload<Message[]> | null> = signal(null);

    private messageReceivedSource: WritableSignal<Payload<Message> | null> = signal(null);

    private messageEditedSource: WritableSignal<Payload<Message> | null> = signal(null);

    private messageDeletedSource: WritableSignal<Payload<Message> | null> = signal(null);

    private newDirectMessagingSource$ = new Subject<Payload<any>>();

    public onConnected = this.onConnectedSource.asReadonly();

    public onReconnected = this.onReconnectedSource.asReadonly();

    public addedServer = this.addedServerSource.asReadonly();

    public updatedServer = this.updatedServerSource.asReadonly();

    public subscriptionDeleted = this.subscriptionDeletedSource.asReadonly();

    public channelCreated = this.channelCreatedSource.asReadonly();

    public channelUpdated = this.channelUpdatedSource.asReadonly();

    public channelDeleted = this.channelDeletedSource.asReadonly();

    public userUpdated = this.userUpdatedSource.asReadonly();

    public invitations = this.invitationsSource.asReadonly();

    public newInvitation = this.newInvitationSource.asReadonly();

    public updatedInvitation = this.updatedInvitationSource.asReadonly();

    public selfAddedToChat = this.selfAddedToChatSource.asReadonly();

    public allMessagesReceived = this.allMessagesReceivedSource.asReadonly();

    public messageReceived = this.messageReceivedSource.asReadonly();

    public messageEdited = this.messageEditedSource.asReadonly();

    public messageDeleted = this.messageDeletedSource.asReadonly();

    /**
     * Creates an instance of ChatService.
     * @memberof ChatService
     */
    constructor(
        private storageService: StorageService,
        private notifyService: NotifyService
    ) {
        effect(() => {
            let token = this.storageService.tokenInfo();
            if (token) {
                this.disposeConnection();
                this.initializeConnection(token);
            }
        });
    }

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
     * This method will process the data sent by a server message and if the data is not of instance
     * BurstChatError then the next method will be called from the provided subject.
     * @private
     * @template T The success type contained within the payload.
     * @param {(Payload<T | BurstChatError>)} payload The payload sent fron the signal server.
     * @param {Subject<Payload<T>>} source The subject to be executed.
     * @memberof ChatService
     */
    private ProcessMessage<T>(payload: Payload<T | BurstChatError>, source: WritableSignal<Payload<T>>): void {
        const error = tryParseError(payload.content);
        if (!error) {
            source.set(payload as Payload<T>);
        } else {
            this.notifyService.notifyError(error);
        }
    }

    /**
     * This method will process the data sent by a server message and if the data is not of instance
     * BurstChatError then the next method will be called from the provided signal.
     * @private
     * @template T The success type contained within the payload.
     * @param {T | BurstChatError} data The data sent fron the server message.
     * @param {Subject<T>} source The subject to be executed.
     * @memberof ChatService
     */
    private ProcessRawMessage<T>(data: T | BurstChatError, source: WritableSignal<T>): void {
        const error = tryParseError(data);
        if (!error) {
            source.set(data as T);
        } else {
            this.notifyService.notifyError(error);
        }
    }

    /**
     * Establishes a new connection to the chat hub and registers all required callbacks.
     * @private
     * @memberof ChatService
     */
    private initializeConnection(tokenInfo: TokenInfo): void {
        try {
            const builder = new HubConnectionBuilder();
            builder
                .withUrl(`${environment.signalUrl}/chat`, {
                    accessTokenFactory: () => tokenInfo.accessToken
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

        this.connection.onreconnected(_connectionId => this.onReconnectedSource.set(true));

        this.connection.on('addedServer', data => this.ProcessRawMessage(data, this.addedServerSource));

        this.connection.on('updatedServer', data => this.ProcessRawMessage(data, this.updatedServerSource));

        this.connection.on('subscriptionDeleted', data => this.ProcessRawMessage(data, this.subscriptionDeletedSource));

        this.connection.on('channelCreated', data => this.ProcessRawMessage(data, this.channelCreatedSource));

        this.connection.on('channelUpdated', data => this.ProcessRawMessage(data, this.channelUpdatedSource));

        this.connection.on('channelDeleted', data => this.ProcessRawMessage(data, this.channelDeletedSource));

        this.connection.on('invitations', data => this.ProcessRawMessage(data, this.invitationsSource));

        this.connection.on('userUpdated', data => this.ProcessRawMessage(data, this.userUpdatedSource));

        this.connection.on('newInvitation', data => this.ProcessRawMessage(data, this.newInvitationSource));

        this.connection.on('updatedInvitation', data => this.ProcessRawMessage(data, this.updatedInvitationSource));

        this.connection.on('selfAddedToPrivateGroup', () => {
            setTimeout(() => this.selfAddedToChatSource.set(true), 500);
        });

        this.connection.on('allPrivateGroupMessagesReceived', data => this.ProcessMessage(data, this.allMessagesReceivedSource));

        this.connection.on('privateGroupMessageReceived', data => this.ProcessMessage(data, this.messageReceivedSource));

        this.connection.on('privateGroupMessageEdited', data => this.ProcessMessage(data, this.messageEditedSource));

        this.connection.on('privateGroupMessageDeleted', data => this.ProcessMessage(data, this.messageDeletedSource));

        this.connection.on('selfAddedToChannel', () => {
            setTimeout(() => this.selfAddedToChatSource.set(true), 500);
        });

        this.connection.on('allChannelMessagesReceived', data => this.ProcessMessage(data, this.allMessagesReceivedSource));

        this.connection.on('channelMessageReceived', data => this.ProcessMessage(data, this.messageReceivedSource));

        this.connection.on('channelMessageEdited', data => this.ProcessMessage(data, this.messageEditedSource));

        this.connection.on('channelMessageDeleted', data => this.ProcessMessage(data, this.messageDeletedSource));

        this.connection.on('selfAddedToDirectMessaging', () => {
            setTimeout(() => this.selfAddedToChatSource.set(true), 500);
        });

        this.connection.on('newDirectMessaging', data => {
            this.ProcessSignal(data, this.newDirectMessagingSource$);
            setTimeout(() => this.selfAddedToChatSource.set(true), 500);
        });

        this.connection.on('allDirectMessagesReceived', data => this.ProcessMessage(data, this.allMessagesReceivedSource));

        this.connection.on('directMessageReceived', data => this.ProcessMessage(data, this.messageReceivedSource));

        this.connection.on('directMessageEdited', data => this.ProcessMessage(data, this.messageEditedSource));

        this.connection.on('directMessageDeleted', data => this.ProcessMessage(data, this.messageDeletedSource));

        this.connection
            .start()
            .then(() => this.onConnectedSource.set(true))
            .catch(error => console.warn(error));
    }

    /**
     * Closes the active connection and executes any necessary clean up code.
     * @private
     * @memberof ChatService
     */
    private disposeConnection(): void {
        if (this.connection && this.connection.state === HubConnectionState.Connected) {
            this.connection.stop();
            this.connection = undefined;
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
                .catch(error => console.warn(error));
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
                .catch(error => console.warn(error));
        }
    }

    /**
     * Sends a message to all associated connections of a server, that its info was updated.
     * @param {number} serverId The id of the target server.
     * @memberod ChatService
     */
    public updateServerInfo(serverId: number) {
        if (this.connection) {
            this.connection
                .invoke('updateServerInfo', serverId)
                .catch(error => console.warn(error));
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
                .catch(error => console.warn(error));
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
                .catch(err => console.warn(err));
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
                .catch(err => console.warn(err));
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
                .catch(err => console.warn(err));
        }
    }

    /**
     * Sends a signal to other connections/groups to update the user's info.
     * @memberof ChatService
     */
    public updateMyInfo() {
        if (this.connection) {
            this.connection
                .invoke('updateMyInfo')
                .catch(err => console.warn(err));
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
                .catch(error => console.warn(error));
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
                .catch(error => console.warn(error));
        }
    }

    /**
     * Updates the invitation sent to user.
     * @param {Invitation} invitation The modified invitation details.
     * @memberof ChatService
     */
    public updateInvitation(id: number, accepted: boolean) {
        if (this.connection) {
            this.connection
                .invoke('updateInvitation', id, accepted)
                .catch(error => console.warn(error));
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
                .catch(error => console.warn(error));
        }
    }

    /**
     * Signals for all messages of a chat to be received.
     * @param {ChatConnectionOptions} options The options to be used for the proper call to the signal server.
     * @param {string} searchTerm The search term based on which the messages will be filtered.
     * @param {number} lastMessageId The last message id of which all messages will preceed.
     * @memberof ChatService
     */
    public getAllMessages(options: ChatConnectionOptions, searchTerm?: string, lastMessageId?: number) {
        if (this.connection && options) {
            let methodName = '';
            let args = [];

            if (options instanceof PrivateGroupConnectionOptions) {
                methodName = 'getAllPrivateGroupMessages';
                args = [options.id];
            } else if (options instanceof ChannelConnectionOptions) {
                methodName = 'getAllChannelMessages';
                args = [options.id, searchTerm, lastMessageId];
            } else {
                methodName = 'getAllDirectMessages';
                args = [options.id, searchTerm, lastMessageId];
            }

            this.connection
                .invoke(methodName, ...args)
                .catch(error => console.warn(error));
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
                .catch(error => console.warn(error));
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
                .catch(error => console.warn(error));
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
                .catch(error => console.warn(error));
        }
    }

}
