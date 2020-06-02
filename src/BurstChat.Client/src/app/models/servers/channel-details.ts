import { Message } from 'src/app/models/chat/message';
import { User } from 'src/app/models/user/user';

/**
 * This interface contains the detailed information about a channel.
 * @interface ChannelDetails
 */
export interface ChannelDetails {

    id: number;
    channelId: number;
    users: User[];
    messages: Message[];
}
