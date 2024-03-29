import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { DirectMessagingComponent } from './direct-messaging.component';

describe('DirectMessagingComponent', () => {
  let component: DirectMessagingComponent;
  let fixture: ComponentFixture<DirectMessagingComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ DirectMessagingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DirectMessagingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
