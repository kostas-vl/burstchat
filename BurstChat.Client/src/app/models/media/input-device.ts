/**
 * This class exposes information about an audio input device.
 * @export
 * @class InputDevice
 */
export class InputDevice {

    /**
     * Creates a new instance of InputDevice.
     * @memberof InputDevice.
     */
    constructor(
        public deviceId: string,
        public groupId: string,
        public label: string
    ) { }
}

