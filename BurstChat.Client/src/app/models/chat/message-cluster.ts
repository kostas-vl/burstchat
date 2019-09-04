import { User } from 'src/app/models/user/user';
import { Message } from 'src/app/models/chat/message';

/**
 * This interface contains a cluster of messages that was sent by the same user.
 * @export
 * @interface MessageCluster
 */
export interface MessageCluster {

    user: User;
    datePosted: Date | string;
    messages: Message[];

}
