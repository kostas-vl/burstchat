import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ImageCroppedEvent, ImageCropperModule } from 'ngx-image-cropper';

@Component({
    selector: 'burst-image-crop',
    templateUrl: './image-crop.component.html',
    styleUrl: './image-crop.component.scss',
    standalone: true,
    imports: [ImageCropperModule]
})
export class ImageCropComponent implements OnInit {

    public imageChangedEvent: any = '';

    public preview?: string;

    @Output()
    public imageCropped = new EventEmitter<string>();

    /**
     * Creates a new instance of ImageCropComponent.
     * @memberof ImageCropComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof ImageCropComponent
     */
    public ngOnInit() { }


    /**
     * Handles the file input change event.
     * @param {any} event The event args.
     * @memberof ImageCropComponent
     */
    public onFileChangeEvent(event: any): void {
        this.imageChangedEvent = event;
    }

    /**
     * Handles the image cropped event.
     * @param {ImageCroppedEvent} event The event args.
     * @memberof ImageCropComponent
     */
    public onImageCropped(event: ImageCroppedEvent) {
        this.preview = event.base64;
        this.imageCropped.emit(event.base64);
    }

 }
