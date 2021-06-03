import { Component, OnInit, Input } from '@angular/core';

/**
 * This class represents an angular component that displays a users avatar on screen.
 * @export
 * @class AvatarComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-avatar',
    templateUrl: './avatar.component.html',
    styleUrls: ['./avatar.component.scss']
})
export class AvatarComponent implements OnInit {

    @Input()
    public name?: string;

    @Input()
    public avatar?: string;

    @Input()
    public size: 'sm' | 'lg' | 'xlg' | 'planetSize' = 'sm';

    @Input()
    public color: 'accent' | 'accent-light' | 'success' | 'danger' | 'warning' = 'accent-light';

    public get visible() {
        return this.avatar?.length > 0 && this.avatar !== '\\x';
    }

    public get initials() {
        const words = this.name?.split(' ') ?? [];
        return words.reduce((acc, n) => acc + n[0]?.toUpperCase(), '');
    }

    /**
     * Creates a new instance of AvatarComponent.
     * @memberof AvatarComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     */
    public ngOnInit() { }

}
