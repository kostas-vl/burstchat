import { TestBed } from '@angular/core/testing';

import { DirectMessagingService } from './direct-messaging.service';

describe('DirectMessagingService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: DirectMessagingService = TestBed.get(DirectMessagingService);
    expect(service).toBeTruthy();
  });
});
