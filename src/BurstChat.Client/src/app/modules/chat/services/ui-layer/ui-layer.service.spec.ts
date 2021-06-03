import { TestBed } from '@angular/core/testing';

import { UiLayerService } from './ui-layer.service';

describe('UiLayerService', () => {
  let service: UiLayerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UiLayerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
