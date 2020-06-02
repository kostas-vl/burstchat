/**
 * This interface exposes properties that describe a link sent by a user in a chat.
 * @export
 * @interface Link
 */
export interface Link {

    id: number;
    url: string;
    dateCreated: Date | string;

}
