import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ChatDirectComponent } from './chat-direct.component';

describe('ChatDirectComponent', () => {
  let component: ChatDirectComponent;
  let fixture: ComponentFixture<ChatDirectComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatDirectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatDirectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
