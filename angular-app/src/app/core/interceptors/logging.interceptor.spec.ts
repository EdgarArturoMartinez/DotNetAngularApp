import { describe, it, expect, vi, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { loggingInterceptor } from './logging.interceptor';
import { LoggingService } from '../services/logging.service';

describe('LoggingInterceptor', () => {
  let httpClient: HttpClient;
  let httpMock: HttpTestingController;
  let loggerSpy: { debug: ReturnType<typeof vi.fn>; warn: ReturnType<typeof vi.fn>; error: ReturnType<typeof vi.fn> };

  beforeEach(() => {
    loggerSpy = {
      debug: vi.fn(),
      warn: vi.fn(),
      error: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([loggingInterceptor])),
        provideHttpClientTesting(),
        { provide: LoggingService, useValue: loggerSpy }
      ]
    });

    httpClient = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should log outgoing requests at debug level', () => {
    httpClient.get('/api/products').subscribe();

    const req = httpMock.expectOne('/api/products');
    req.flush([]);

    expect(loggerSpy.debug).toHaveBeenCalled();
  });

  it('should log successful responses at debug level', () => {
    httpClient.get('/api/categories').subscribe();

    const req = httpMock.expectOne('/api/categories');
    req.flush({ id: 1, name: 'Vegetables' });

    // request log + response log = 2 debug calls
    expect(loggerSpy.debug).toHaveBeenCalledTimes(2);
  });

  it('should not log requests to /api/clientlogs endpoint', () => {
    httpClient.post('/api/clientlogs', []).subscribe();

    const req = httpMock.expectOne('/api/clientlogs');
    req.flush({ received: 0 });

    expect(loggerSpy.debug).not.toHaveBeenCalled();
  });

  it('should log 4xx errors as warnings', () => {
    httpClient.get('/api/products/999').subscribe({
      error: () => { /* expected */ }
    });

    const req = httpMock.expectOne('/api/products/999');
    req.flush({ message: 'Not found' }, { status: 404, statusText: 'Not Found' });

    expect(loggerSpy.warn).toHaveBeenCalled();
  });

  it('should log 5xx errors as errors', () => {
    httpClient.get('/api/products').subscribe({
      error: () => { /* expected */ }
    });

    const req = httpMock.expectOne('/api/products');
    req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });

    expect(loggerSpy.error).toHaveBeenCalled();
  });
});
