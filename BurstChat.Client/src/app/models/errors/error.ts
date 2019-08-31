/**
 * This class represents an error sent by the BurstChat API or Signal Server about an
 * unsuccessful operation.
 */
export class BurstChatError {

    public level = '';
    public type = '';
    public message = '';

}

/**
 * This function will try to parse an instance of any kind to an error if its applicable.
 * It will return null if the parsing fails.
 * @param {any} data The data that will be used.
 * @returns {BurstChatError | null} An error instance or null based on the success of the operation.
 */
export function tryParseError(data: any): BurstChatError | null {
    try {
        const error = Object.assign(new Error(), data);
        if (error && error.level && error.type && error.message) {
            return error;
        } else {
            return null;
        }
    } catch {
        return null;
    }
}
