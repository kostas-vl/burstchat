import { User } from 'src/app/models/user/user';
import { Link } from 'src/app/models/chat/link';

/**
 * This interface exposes the base contract of the data required to display a message on screen.
 * @export
 * @interface Message
 */
export interface Message {

    id: number;
    userId: number;
    user: User;
    content: string;
    datePosted: Date | string;
    edited: boolean;
    links: Link[];

}
