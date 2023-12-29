import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ShimmerListComponent } from './shimmer-list.component';

describe('ShimmerListComponent', () => {
  let component: ShimmerListComponent;
  let fixture: ComponentFixture<ShimmerListComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ShimmerListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShimmerListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
