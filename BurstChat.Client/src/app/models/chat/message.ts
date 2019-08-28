/**
 * This interface exposes the base contract of the data required to display a message on screen.
 * @export
 * @interface Message
 */
export interface Message {

    id: number;
    userId: number;
    content: string;
    datePosted: Date | string;
    edited: boolean;

}
