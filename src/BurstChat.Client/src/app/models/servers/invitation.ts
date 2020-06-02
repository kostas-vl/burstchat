import { Server } from 'src/app/models/servers/server';
import { User } from 'src/app/models/user/user';

/**
 * This interface contains all properties that represent a server invitation sent to a user.
 * @export
 * @interface Invitation
 */
export interface Invitation {

    id: number;
    serverId: number;
    server: Server;
    userId: number;
    user: User;
    accepted: boolean;
    declined: boolean;
    dateUpdated: Date | string | null;
    dateCreated: Date | string;

}
