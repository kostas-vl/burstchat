import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
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

    private inputStreamSub?: Subscription;

    private audio = new Audio();

    public testingAudio = false;

    public inputDevices: InputDevice[] = [];

    public selectedInputDevice?: string;

    public outputDevices: OutputDevice[] = [];

    public selectedOutputDevice?: string;

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

        this.inputStreamSub = this
            .mediaService
            .inputStream
            .subscribe(stream => {
                if (stream) {
                    this.startTestAudio(stream);
                }
            });
    }

    /**
     * Executes any necessary code for the destruction of the component.
     * @memberof EditUserMediaComponent
     */
    public ngOnDestroy() {
        this.inputDevicesSub?.unsubscribe();
        this.outputDevicesSub?.unsubscribe();
        this.inputStreamSub?.unsubscribe();
    }

    /**
     * Assigns to the test audio player the provided stream instance.
     * @private
     * @param {MediaStream} stream The media stream instance
     * @memberof EditUserMediaComponent
     */
    private startTestAudio(stream: MediaStream) {
        this.audio.srcObject = stream;
        this.audio.play();
        this.testingAudio = true;
    }

    /**
     * Stops the current playing stream in the test audio player.
     * @private
     * @memberof EditUserMediaComponent
     */
    private stopTestAudio() {
        const stream = this.audio.srcObject as MediaStream;
        const tracks = stream?.getTracks();

        for (const track of tracks) {
            track.stop();
        }

        this.audio.srcObject = null;
        this.audio.pause();
        this.testingAudio = false;
    }

    /**
     * Handles the input device select box change event.
     * @memberof EditUserMediaComponent
     */
    public onSelectedInputDeviceChange(value: string) {
        this.selectedInputDevice = value;
    }

    /**
     * Handles the output device select box change event.
     * @memberof EditUserMediaComponent
     */
    public onSelectedOutputDeviceChange(value: string) {
        this.selectedOutputDevice = value;
    }

    /**
     * Handles the test input button click event.
     * @memberof EditUserMediaComponent
     */
    public onTestInput() {
        if (!this.testingAudio) {
            this.mediaService.activateInput(this.selectedInputDevice);
        } else {
            this.stopTestAudio();
        }
    }

}

