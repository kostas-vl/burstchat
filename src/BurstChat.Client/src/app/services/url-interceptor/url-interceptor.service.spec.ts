import { TestBed } from '@angular/core/testing';

import { UrlInterceptorService } from './url-interceptor.service';

describe('UrlInterceptorService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UrlInterceptorService = TestBed.get(UrlInterceptorService);
    expect(service).toBeTruthy();
  });
});
