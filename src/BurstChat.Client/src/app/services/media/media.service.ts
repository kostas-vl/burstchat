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

  private inputStreamSource = new BehaviorSubject<MediaStream | null>(null);

  public inputDevices = this.inputDevicesSource.asObservable();

  public outputDevices = this.outputDevicesSource.asObservable();

  public inputStream = this.inputStreamSource.asObservable();

  /**
   * Creates a new instance of MediaService.
   * @memberof MediaService
   */
  constructor() {
    this.enumerateDevices();
    this.subscribeToDeviceChanges();
  }

  /**
   * Enumerates all available media devices and updated the input and output device sources
   * accordingly.
   * @private
   * @memberof MediaService
   */
  private enumerateDevices() {
    if (!navigator.mediaDevices) return;

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
              outputDevices.push(output);
              continue;
            }
          }

          this.inputDevicesSource.next(inputDevices);
          this.outputDevicesSource.next(outputDevices);
        })
        .catch(error => {
          console.warn(error);
        });
    } catch (ex) {
      console.warn(ex);
    }
  }

  /**
   * Assigns a new event listener function that will update all input
   * and output devices when a device change on the user's system has
   * occured.
   * @private
   * @memberof MediaService
   */
  private subscribeToDeviceChanges() {
    if (!navigator.mediaDevices) return;

    navigator
      .mediaDevices
      .addEventListener('devicechange', event => {
        this.enumerateDevices();
      });
  }

  /**
   * Enables the activation of a new stream for a device with the provided id.
   * @param {string} deviceId The id of the input device
   * @memberof MediaService
   */
  public activateInput(deviceId: string) {
    if (!navigator.mediaDevices) return;

    try {
      const constraints: MediaStreamConstraints = {
        audio: {
          deviceId: deviceId
        }
      };

      navigator
        .mediaDevices
        .getUserMedia(constraints)
        .then(stream => {
          this.inputStreamSource.next(stream);
        })
        .catch(error => {
          console.warn(error);
        });
    } catch (ex) {
      console.warn(ex);
    }
  }

}
