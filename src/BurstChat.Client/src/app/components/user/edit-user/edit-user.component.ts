import { Component, OnInit } from '@angular/core';
import { CardComponent } from 'src/app/components/shared/card/card.component';
import { CardBodyComponent } from 'src/app/components/shared/card-body/card-body.component';
import { EditUserInfoComponent } from 'src/app/components/user/edit-user-info/edit-user-info.component';
import { EditUserInvitationsComponent } from 'src/app/components/user/edit-user-invitations/edit-user-invitations.component';
import { EditUserMediaComponent } from 'src/app/components/user/edit-user-media/edit-user-media.component';

/**
 * This class represents an angular component for dispaying and modifying user information.
 * @export
 * @class EditUserComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-edit-user',
    templateUrl: './edit-user.component.html',
    styleUrl: './edit-user.component.scss',
    standalone: true,
    imports: [
        CardComponent,
        CardBodyComponent,
        EditUserInfoComponent,
        EditUserInvitationsComponent,
        EditUserMediaComponent
    ]
})
export class EditUserComponent implements OnInit {

    /**
     * Creates an instance of EditUserComponent.
     * @memberof EditUserComponent
     */
    constructor() { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditUserComponent
     */
    public ngOnInit() { }

}
