import { Channel } from 'src/app/models/servers/channel';
import { User } from 'src/app/models/user/user';

/**
 * This interface contains information about a BurstChat server.
 * @interface Server
 */
export interface Server {

    id: number;
    name: string;
    dateCreated: Date | string;
    channels: Channel[];
    subscribedUsers: User[];

}

