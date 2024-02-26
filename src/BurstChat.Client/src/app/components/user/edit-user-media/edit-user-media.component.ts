import { Component, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MediaService } from 'src/app/services/media/media.service';

/**
 * This class represents an angular component that displays to the user the available media
 * input and output, on the current device, and a way to configure them.
 * @exports
 * @class EditUserMediaComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-edit-user-media',
    templateUrl: './edit-user-media.component.html',
    styleUrl: './edit-user-media.component.scss',
    standalone: true,
    imports: [FormsModule]
})
export class EditUserMediaComponent {

    private audio = new Audio();

    public testingAudio = false;

    public inputDevices = this.mediaService.inputDevices;

    public selectedInputDevice?: string;

    public outputDevices = this.mediaService.outputDevices;

    public selectedOutputDevice?: string;

    /**
     * Creates a new instance of EditUserMediaComponent.
     * @memberof EditUserMediaComponent
     */
    constructor(private mediaService: MediaService) {
        effect(() => {
            const stream = this.mediaService.inputStream();
            if (stream) {
                this.startTestAudio(stream);
            }
        })
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

