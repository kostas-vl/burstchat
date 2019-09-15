import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditUserInvitationsComponent } from './edit-user-invitations.component';

describe('EditUserInvitationsComponent', () => {
  let component: EditUserInvitationsComponent;
  let fixture: ComponentFixture<EditUserInvitationsComponent>;

  beforeEach(async(() => {
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
