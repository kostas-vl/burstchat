import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MessageDeleteDialogComponent } from './message-delete-dialog.component';

describe('MessageDeleteDialogComponent', () => {
  let component: MessageDeleteDialogComponent;
  let fixture: ComponentFixture<MessageDeleteDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MessageDeleteDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MessageDeleteDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
