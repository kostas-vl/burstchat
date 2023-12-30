import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditUserInvitationsComponent } from './edit-user-invitations.component';

describe('EditUserInvitationsComponent', () => {
  let component: EditUserInvitationsComponent;
  let fixture: ComponentFixture<EditUserInvitationsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditUserInvitationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditUserInvitationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
