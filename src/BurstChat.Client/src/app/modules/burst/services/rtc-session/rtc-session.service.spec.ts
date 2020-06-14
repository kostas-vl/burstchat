import { TestBed } from '@angular/core/testing';

import { RtcSessionService } from './rtc-session.service';

describe('RtcSessionService', () => {
  let service: RtcSessionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RtcSessionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
