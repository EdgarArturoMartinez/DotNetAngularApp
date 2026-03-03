import { ErrorHandler, Injectable, inject } from '@angular/core';
import { LoggingService } from '../services/logging.service';

/**
 * Global error handler that catches all unhandled exceptions
 * and routes them through the LoggingService.
 *
 * Replaces Angular's default ErrorHandler which only writes to console.error.
 * This ensures every uncaught error is:
 *  1. Logged to the browser console with structured formatting
 *  2. Forwarded to the backend for aggregation in Serilog
 */
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private logger = inject(LoggingService);

  handleError(error: unknown): void {
    const err = error instanceof Error ? error : new Error(String(error));

    this.logger.fatal('GlobalErrorHandler', 'Unhandled error caught', err);
  }
}
