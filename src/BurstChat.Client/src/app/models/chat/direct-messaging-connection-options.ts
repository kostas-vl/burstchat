import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';
import { DirectMessaging } from 'src/app/models/user/direct-messaging';

/**
 * This class holds the options required for establishing a realtime chat connection for direct messaging.
 * @export
 * @class DirectMessagingConnectionOptions
 * @implements {ChatConnectionOptions}
 */
export class DirectMessagingConnectionOptions implements ChatConnectionOptions {

    public signalGroup = '';
    public name = '';
    public id = 0;
    public directMessaging: DirectMessaging = null;

}
