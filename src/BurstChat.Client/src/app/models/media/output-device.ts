/**
 * This class exposes information about an audio output device.
 * @export
 * @class OutputDevice
 */
export class OutputDevice {

    /**
     * Creates a new instance of OutputDevice.
     * @memberof OutputDevice
     */
    constructor(
        public deviceId: string,
        public groupId: string,
        public label: string
    ) { }

}

