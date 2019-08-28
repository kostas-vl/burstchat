import { ChannelDetails } from 'src/app/models/servers/channel-details';

/**
 * This interface contains the base information about a channel.
 * @interface Channel
 */
export interface Channel {

    id: number;
    serverId: number;
    name: string;
    isPublic: boolean;
    dateCreated: Date | string;
    details: ChannelDetails;

}
