import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditServerInfoComponent } from './edit-server-info.component';

describe('EditServerInfoComponent', () => {
  let component: EditServerInfoComponent;
  let fixture: ComponentFixture<EditServerInfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditServerInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditServerInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
