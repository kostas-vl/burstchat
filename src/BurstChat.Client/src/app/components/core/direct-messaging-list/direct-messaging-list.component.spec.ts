import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { DirectMessagingListComponent } from './direct-messaging-list.component';

describe('DirectMessagingListComponent', () => {
  let component: DirectMessagingListComponent;
  let fixture: ComponentFixture<DirectMessagingListComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ DirectMessagingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DirectMessagingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
