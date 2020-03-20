import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { InputDevice } from 'src/app/models/media/input-device';
import { OutputDevice } from 'src/app/models/media/output-device';

/**
 * This class represents an angular services that provides functionality for controlling media devices
 * for the user.
 * @export
 * @class MediaService
 */
@Injectable()
export class MediaService {

    private inputDevicesSource = new BehaviorSubject<InputDevice[]>([]);

    private outputDevicesSource = new BehaviorSubject<OutputDevice[]>([]);

    public inputDevices = this.inputDevicesSource.asObservable();

    public outputDevices = this.outputDevicesSource.asObservable();

    /**
     * Creates a new instance of MediaService.
     * @memberof MediaService
     */
    constructor() {
        this.enumerateDevices();
    }

    /**
     * Enumerates all available media devices and updated the input and output device sources
     * accordingly.
     * @private
     * @memberof MediaService
     */
    private enumerateDevices() {
        try {
            navigator
                .mediaDevices
                .enumerateDevices()
                .then(devices => {
                    let inputIndex = 1;
                    let inputDevices = [];
                    let outputIndex = 1;
                    let outputDevices = [];

                    for (let device of devices) {
                        if (device.kind === 'audioinput') {
                            const input = new InputDevice(
                                device.deviceId,
                                device.groupId,
                                device.label || `Input source #${inputIndex++}`
                            );
                            inputDevices.push(input);
                            continue;
                        }

                        if (device.kind === 'audiooutput') {
                            const output = new OutputDevice(
                                device.deviceId,
                                device.groupId,
                                device.label || `Output source #${outputIndex++}`
                            );
                            outputDevices.push(device);
                            continue;
                        }
                    }

                    this.inputDevicesSource.next(inputDevices);
                    this.outputDevicesSource.next(outputDevices);
                }, error => {
                    console.warn(error);
                });
        } catch (ex) {
            console.warn(ex);
        }
    }

}
