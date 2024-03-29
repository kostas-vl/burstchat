import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { OngoingCallComponent } from './ongoing-call.component';

describe('OngoingCallComponent', () => {
  let component: OngoingCallComponent;
  let fixture: ComponentFixture<OngoingCallComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ OngoingCallComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OngoingCallComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
