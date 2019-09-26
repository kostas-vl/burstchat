import { ChatConnectionOptions } from 'src/app/models/chat/chat-connection-options';

/**
 * This class holds the options required for establishing a realtime chat connection to a channel.
 * @class ChannelConnectionOptions
 */
export class ChannelConnectionOptions implements ChatConnectionOptions {

    public signalGroup = '';
    public name = '';
    public id = 0;

}

