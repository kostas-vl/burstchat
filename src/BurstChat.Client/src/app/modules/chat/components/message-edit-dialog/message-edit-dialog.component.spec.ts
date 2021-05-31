import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MessageEditDialogComponent } from './message-edit-dialog.component';

describe('MessageEditDialogComponent', () => {
  let component: MessageEditDialogComponent;
  let fixture: ComponentFixture<MessageEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MessageEditDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MessageEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
