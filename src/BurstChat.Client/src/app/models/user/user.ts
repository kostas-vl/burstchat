/**
 * This interface contains information about a user.
 * @export
 * @interface User
 */
export interface User {

    id: number;
    email: string;
    name: string;
    messages: any[];
    subscribedServers: any[];
    dateCreated: Date | string;

}

