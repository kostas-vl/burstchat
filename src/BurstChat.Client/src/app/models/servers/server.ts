import { Channel } from 'src/app/models/servers/channel';
import { Subscription } from 'src/app/models/servers/subscription';

/**
 * This interface contains information about a BurstChat server.
 * @interface Server
 */
export interface Server {

    id: number;
    name: string;
    dateCreated: Date | string;
    channels: Channel[];
    subscriptions: Subscription[];

}

/**
 * This is the base class of a BurstChat server that implements the Server interface.
 * @class BurstChatServer
 */
export class BurstChatServer implements Server {

    public id = 0;
    public name = '';
    public dateCreated = new Date();
    public channels = [];
    public subscriptions = [];

}

