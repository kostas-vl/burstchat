import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';

/**
 * This class holds the options required for establishing a realtime chat connection to a private group.
 * @class PrivateGroupConnectionOptions
 */
export class PrivateGroupConnectionOptions implements ChatConnectionOptions {

    public signalGroup = '';
    name = '';
    public id = 0;

}

