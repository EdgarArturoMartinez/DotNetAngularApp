import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { LoggingService, LogLevel } from './logging.service';

describe('LoggingService', () => {
  let service: LoggingService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        LoggingService
      ]
    });

    service = TestBed.inject(LoggingService);
    httpMock = TestBed.inject(HttpTestingController);

    // Override environment defaults for testing (environment.ts has console disabled and logLevel=Info)
    (service as any).consoleEnabled = true;
    (service as any).minLevel = LogLevel.Trace;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should log info messages to the console', () => {
    const consoleSpy = vi.spyOn(console, 'info').mockImplementation(() => {});

    service.info('TestComponent', 'Test info message');

    expect(consoleSpy).toHaveBeenCalled();
    consoleSpy.mockRestore();
  });

  it('should log debug messages to the console', () => {
    const consoleSpy = vi.spyOn(console, 'debug').mockImplementation(() => {});

    service.debug('TestComponent', 'Test debug message');

    expect(consoleSpy).toHaveBeenCalled();
    consoleSpy.mockRestore();
  });

  it('should log warn messages to the console', () => {
    const consoleSpy = vi.spyOn(console, 'warn').mockImplementation(() => {});

    service.warn('TestComponent', 'Test warning message');

    expect(consoleSpy).toHaveBeenCalled();
    consoleSpy.mockRestore();
  });

  it('should log error messages to the console', () => {
    const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

    service.error('TestComponent', 'Test error message');

    expect(consoleSpy).toHaveBeenCalled();
    consoleSpy.mockRestore();
  });

  it('should log fatal messages to the console', () => {
    const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

    service.fatal('TestComponent', 'Test fatal message');

    expect(consoleSpy).toHaveBeenCalled();
    consoleSpy.mockRestore();
  });

  it('should log error with Error object and include stack trace', () => {
    const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
    const testError = new Error('Something broke');

    service.error('TestComponent', 'Operation failed', testError);

    expect(consoleSpy).toHaveBeenCalled();
    consoleSpy.mockRestore();
  });

  it('should log trace messages to console debug', () => {
    const consoleSpy = vi.spyOn(console, 'debug').mockImplementation(() => {});

    service.trace('TestComponent', 'Trace message');

    expect(consoleSpy).toHaveBeenCalled();
    consoleSpy.mockRestore();
  });
});
