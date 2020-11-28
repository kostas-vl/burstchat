import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditUserMediaComponent } from './edit-user-media.component';

describe('EditUserMediaComponent', () => {
  let component: EditUserMediaComponent;
  let fixture: ComponentFixture<EditUserMediaComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditUserMediaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditUserMediaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
