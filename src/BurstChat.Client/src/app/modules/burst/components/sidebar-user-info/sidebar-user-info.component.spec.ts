import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SidebarUserInfoComponent } from './sidebar-user-info.component';

describe('SidebarUserInfoComponent', () => {
  let component: SidebarUserInfoComponent;
  let fixture: ComponentFixture<SidebarUserInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SidebarUserInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SidebarUserInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
