import { describe, it, expect, vi, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { GlobalErrorHandler } from './global-error.handler';
import { LoggingService } from '../services/logging.service';

describe('GlobalErrorHandler', () => {
  let handler: GlobalErrorHandler;
  let loggerSpy: { fatal: ReturnType<typeof vi.fn> };

  beforeEach(() => {
    loggerSpy = { fatal: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        GlobalErrorHandler,
        { provide: LoggingService, useValue: loggerSpy }
      ]
    });

    handler = TestBed.inject(GlobalErrorHandler);
  });

  it('should be created', () => {
    expect(handler).toBeTruthy();
  });

  it('should log Error objects as fatal', () => {
    const error = new Error('Test error');
    handler.handleError(error);
    expect(loggerSpy.fatal).toHaveBeenCalledWith('GlobalErrorHandler', 'Unhandled error caught', error);
  });

  it('should wrap non-Error values in Error objects', () => {
    handler.handleError('String error');
    expect(loggerSpy.fatal).toHaveBeenCalled();
    const callArgs = loggerSpy.fatal.mock.calls[0];
    expect(callArgs[0]).toBe('GlobalErrorHandler');
    expect(callArgs[1]).toBe('Unhandled error caught');
    expect(callArgs[2]).toBeInstanceOf(Error);
  });

  it('should handle null errors gracefully', () => {
    handler.handleError(null);
    expect(loggerSpy.fatal).toHaveBeenCalled();
  });

  it('should handle undefined errors gracefully', () => {
    handler.handleError(undefined);
    expect(loggerSpy.fatal).toHaveBeenCalled();
  });
});
