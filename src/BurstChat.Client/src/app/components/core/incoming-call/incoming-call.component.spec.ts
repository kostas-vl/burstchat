import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { IncomingCallComponent } from './incoming-call.component';

describe('IncomingCallComponent', () => {
  let component: IncomingCallComponent;
  let fixture: ComponentFixture<IncomingCallComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ IncomingCallComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IncomingCallComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
