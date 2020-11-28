import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditServerChannelsComponent } from './edit-server-channels.component';

describe('EditServerChannelsComponent', () => {
  let component: EditServerChannelsComponent;
  let fixture: ComponentFixture<EditServerChannelsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditServerChannelsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditServerChannelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
