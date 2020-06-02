import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';

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

}
