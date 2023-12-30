import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditUserInfoComponent } from './edit-user-info.component';

describe('EditUserInfoComponent', () => {
  let component: EditUserInfoComponent;
  let fixture: ComponentFixture<EditUserInfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditUserInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditUserInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
