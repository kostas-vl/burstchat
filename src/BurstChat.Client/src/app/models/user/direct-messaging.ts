import { Message } from 'src/app/models/chat/message';
import { User } from 'src/app/models/user/user';

/**
 * This interface contains properties that define the available information about a direct messaging
 * chat.
 * @export
 * @interface DirectMessaging
 */
export interface DirectMessaging {

    id: number;
    firstParticipantUserId: number;
    firstParticipantUser: User;
    secondParticipantUserId: number;
    secondParticipantUser: User;
    messages: Message[];

}
