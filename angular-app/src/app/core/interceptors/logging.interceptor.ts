import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { tap, catchError, throwError } from 'rxjs';
import { LoggingService } from '../services/logging.service';

/**
 * HTTP Logging Interceptor
 *
 * Logs outgoing HTTP requests and their responses / errors through the
 * centralized LoggingService.  Placed **after** the JWT interceptor so that
 * timing includes token attachment.
 *
 * What it logs:
 *  - DEBUG: Every outgoing request (method + URL)
 *  - DEBUG: Successful responses with status and elapsed time
 *  - WARN:  4xx client errors (including response body summary)
 *  - ERROR: 5xx server errors and network failures
 *
 * The interceptor skips logging for calls to the `/api/clientlogs` endpoint
 * to avoid recursive log storms.
 */
export const loggingInterceptor: HttpInterceptorFn = (req, next) => {
  const logger = inject(LoggingService);

  // Don't log calls to the client-logs endpoint (avoid recursion)
  if (req.url.includes('/api/clientlogs')) {
    return next(req);
  }

  const startTime = performance.now();

  logger.debug('HttpInterceptor', `→ ${req.method} ${req.url}`);

  return next(req).pipe(
    tap((event) => {
      // HttpResponse events have a 'status' property
      if (event && 'status' in event) {
        const elapsed = Math.round(performance.now() - startTime);
        logger.debug(
          'HttpInterceptor',
          `← ${req.method} ${req.url} [${(event as any).status}] ${elapsed}ms`
        );
      }
    }),
    catchError((error) => {
      const elapsed = Math.round(performance.now() - startTime);
      const status = error?.status ?? 0;
      const statusText = error?.statusText ?? 'Unknown';

      if (status === 0) {
        // Network error or CORS failure
        logger.error('HttpInterceptor', `Network error: ${req.method} ${req.url} [${elapsed}ms]`, error);
      } else if (status >= 500) {
        logger.error('HttpInterceptor', `Server error: ${req.method} ${req.url} [${status} ${statusText}] ${elapsed}ms`, error);
      } else if (status >= 400) {
        logger.warn('HttpInterceptor', `Client error: ${req.method} ${req.url} [${status} ${statusText}] ${elapsed}ms`);
      }

      return throwError(() => error);
    })
  );
};
