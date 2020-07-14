import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Dimensions, ImageCroppedEvent, ImageTransform, base64ToFile } from 'ngx-image-cropper';

@Component({
    selector: 'app-image-crop',
    templateUrl: './image-crop.component.html',
    styleUrls: ['./image-crop.component.scss']
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
