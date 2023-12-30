import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PopupListComponent } from './popup-list.component';

describe('PopupListComponent', () => {
  let component: PopupListComponent;
  let fixture: ComponentFixture<PopupListComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PopupListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PopupListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
