import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditServerUsersComponent } from './edit-server-users.component';

describe('EditServerUsersComponent', () => {
  let component: EditServerUsersComponent;
  let fixture: ComponentFixture<EditServerUsersComponent>;

  beforeEach(async(() => {
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
