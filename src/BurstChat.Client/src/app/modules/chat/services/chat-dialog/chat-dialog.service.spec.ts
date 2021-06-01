import { TestBed } from '@angular/core/testing';

import { ChatDialogService } from './chat-dialog.service';

describe('ChatDialogService', () => {
  let service: ChatDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChatDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
