import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ImageCropComponent } from './image-crop.component';

describe('ImageCropComponent', () => {
  let component: ImageCropComponent;
  let fixture: ComponentFixture<ImageCropComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ImageCropComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImageCropComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
