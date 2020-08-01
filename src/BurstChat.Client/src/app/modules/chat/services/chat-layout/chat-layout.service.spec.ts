import { TestBed } from '@angular/core/testing';

import { ChatLayoutService } from './chat-layout.service';

describe('ChatLayoutService', () => {
  let service: ChatLayoutService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChatLayoutService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
