import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditServerUsersComponent } from './edit-server-users.component';

describe('EditServerUsersComponent', () => {
  let component: EditServerUsersComponent;
  let fixture: ComponentFixture<EditServerUsersComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditServerUsersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditServerUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
