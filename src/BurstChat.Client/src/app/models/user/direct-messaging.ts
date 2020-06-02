import { Message } from 'src/app/models/chat/message';

/**
 * This interface contains properties that define the available information about a direct messaging
 * chat.
 * @export
 * @interface DirectMessaging
 */
export interface DirectMessaging {

    id: number;
    firstParticipantUserId: number;
    secondParticipantUserId: number;
    messages: Message[];

}
