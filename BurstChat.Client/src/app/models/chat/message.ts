/**
 * This interface exposes the base contract of the data required to display a message on screen.
 * @export
 * @interface IMessage
 */
export interface IMessage {

    content: string;
    user: string;
    datePosted: Date | string;
    edited: boolean;

}
