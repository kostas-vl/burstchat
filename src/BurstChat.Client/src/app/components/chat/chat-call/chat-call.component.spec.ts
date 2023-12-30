import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ChatCallComponent } from './chat-call.component';

describe('ChatCallComponent', () => {
  let component: ChatCallComponent;
  let fixture: ComponentFixture<ChatCallComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatCallComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatCallComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
