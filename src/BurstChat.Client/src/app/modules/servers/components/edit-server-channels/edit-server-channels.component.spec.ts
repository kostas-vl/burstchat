import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditServerChannelsComponent } from './edit-server-channels.component';

describe('EditServerChannelsComponent', () => {
  let component: EditServerChannelsComponent;
  let fixture: ComponentFixture<EditServerChannelsComponent>;

  beforeEach(async(() => {
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
