import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { InputDevice } from 'src/app/models/media/input-device';
import { OutputDevice } from 'src/app/models/media/output-device';
import { MediaService } from 'src/app/modules/burst/services/media/media.service';

/**
 * This class represents an angular component that displays to the user the available media
 * input and output, on the current device, and a way to configure them.
 * @exports
 * @class EditUserMediaComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'app-edit-user-media',
    templateUrl: './edit-user-media.component.html',
    styleUrls: ['./edit-user-media.component.scss']
})
export class EditUserMediaComponent implements OnInit, OnDestroy {

    private inputDevicesSub?: Subscription;

    private outputDevicesSub?: Subscription;

    public inputDevices: InputDevice[] = [];

    public outputDevices: OutputDevice[] = [];

    /**
     * Creates a new instance of EditUserMediaComponent.
     * @memberof EditUserMediaComponent
     */
    constructor(private mediaService: MediaService) { }

    /**
     * Executes any necessary start up code for the component.
     * @memberof EditUserMediaComponent
     */
    public ngOnInit() {
        this.inputDevicesSub = this
            .mediaService
            .inputDevices
            .subscribe(devices => this.inputDevices = devices);

        this.outputDevicesSub = this
            .mediaService
            .outputDevices
            .subscribe(devices => this.outputDevices = devices);
    }

    /**
     * Executes any necessary code for the destruction of the component.
     * @memberof EditUserMediaComponent
     */
    public ngOnDestroy() {
        this.inputDevicesSub?.unsubscribe();
        this.outputDevicesSub?.unsubscribe();
    }

    /**
     * Handles the input device select box change event.
     * @param {string} deviceId The id of the selected input device.
     * @memberof EditUserMediaComponent
     */
    public onInputDeviceChange(deviceId: string) {

    }

    /**
     * Handles the output device select box change event.
     * @param {string} deviceId The id of the selected output device.
     * @memberof EditUserMediaComponent
     */
    public onOutputDeviceChange(deviceId: string) {

    }

}

