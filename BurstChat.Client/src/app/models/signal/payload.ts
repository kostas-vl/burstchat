/**
 * This interface is the object sent from the BurstChat signal server that contains information
 * about the signalGroup and its content.
 * * @export
 * @interface Payload
 * @template T The type contained within the payload.
 */
export interface Payload<T> {

    signalGroup: string;
    content: T;

}
