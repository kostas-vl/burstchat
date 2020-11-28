import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditServerComponent } from './edit-server.component';

describe('EditServerComponent', () => {
  let component: EditServerComponent;
  let fixture: ComponentFixture<EditServerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditServerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditServerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
