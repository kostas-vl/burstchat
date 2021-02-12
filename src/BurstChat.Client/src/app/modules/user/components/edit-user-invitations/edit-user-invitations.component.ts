import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Invitation } from 'src/app/models/servers/invitation';
import { ChatService } from 'src/app/modules/burst/services/chat/chat.service';

/**
 * This class represents an angular component that displays to the user a list of server invitations
 * that was sent to him.
 * @export
 * @class EditUserInvitationsComponent
 * @implements {OnInit}
 */
@Component({
    selector: 'burst-edit-user-invitations',
    templateUrl: './edit-user-invitations.component.html',
    styleUrls: ['./edit-user-invitations.component.scss']
})
export class EditUserInvitationsComponent implements OnInit, OnDestroy {

    private subscriptions: Subscription[];

    public invitations: Invitation[] = [];

    public processingQueue: number[] = [];

    /**
     * Creates an instance of EditUserInvitationsComponent.
     * @memberof EditUserInvitationsComponent
     */
    constructor(private chatService: ChatService) { }

    /**
     * Executes any neccessary start up code for the component.
     * @memberof EditUserInvitationsComponent
     */
    public ngOnInit() {
        this.subscriptions = [
            this
                .chatService
                .invitations
                .subscribe(invitations => {
                    this.invitations = invitations;
                }),
            this
                .chatService
                .updatedInvitation
                .subscribe(invite => {
                    const storedInvite = this.invitations.find(entry => entry.serverId === invite.serverId);
                    if (storedInvite) {
                        const index = this.invitations.indexOf(storedInvite);
                        this.invitations.splice(index, 1);
                    }

                    const processingEntryIndex = this.processingQueue.indexOf(storedInvite.id);
                    if (processingEntryIndex !== -1) {
                        this.processingQueue.splice(processingEntryIndex, 1);
                    }
                })
        ];
    }

    /**
     * Executes any neccessary code for the destruction of the component.
     * @memberof EditUserInvitationsComponent
     */
    public ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }

    /**
     * Checks if the provided id is in processing of a remote operation.
     * @param {number} id The id of the invitation.
     * @returns A boolean specifying the state of the entry.
     * @memberof EditUserInvitationsComponent
     */
    public isProcessing(id: number) {
        return this.processingQueue.some(e => e === id);
    }

    /**
     * Handles the accept button click event.
     * @param {Invitation} invite The invitation instance sent that will be accepted.
     * @memberof EditUserInvitationsComponent
     */
    public onAccept(invite: Invitation) {
        if (invite) {
            invite.accepted = true;
            this.chatService.updateInvitation(invite.id, true);
        }
    }

    /**
     * Handles the decline button click event.
     * @param {Invitation} invite The invitation instance sent that will be declined.
     * @memberof EditUserInvitationsComponent
     */
    public onDecline(invite: Invitation) {
        if (invite) {
            invite.declined = true;
            this.chatService.updateInvitation(invite.id, false);
            this.processingQueue.push(invite.id);
        }
    }

}
