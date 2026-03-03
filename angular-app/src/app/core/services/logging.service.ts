import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

/**
 * Log levels ordered by severity (lowest to highest).
 */
export enum LogLevel {
  Trace = 0,
  Debug = 1,
  Info = 2,
  Warn = 3,
  Error = 4,
  Fatal = 5,
  Off = 6
}

/**
 * A single log entry emitted by the LoggingService.
 */
export interface LogEntry {
  level: string;
  message: string;
  source: string;
  timestamp: string;
  stackTrace?: string;
}

/**
 * Centralized logging service for the Angular application.
 *
 * Features:
 *  - Configurable minimum log level (via environment)
 *  - Console output with colour-coded levels
 *  - Automatic batched forwarding to the backend `/api/clientlogs` endpoint
 *  - Source tracking (component / service name)
 *
 * Usage:
 * ```ts
 * private logger = inject(LoggingService);
 *
 * this.logger.info('ProductList', 'Loaded 25 products');
 * this.logger.error('AuthService', 'Login failed', error);
 * ```
 */
@Injectable({ providedIn: 'root' })
export class LoggingService {
  private http = inject(HttpClient);
  private buffer: LogEntry[] = [];
  private flushTimer: ReturnType<typeof setTimeout> | null = null;

  /** Minimum level to emit (anything below is suppressed). */
  private minLevel: LogLevel = environment.logLevel ?? LogLevel.Debug;

  /** Whether to write to the browser console. */
  private consoleEnabled: boolean = environment.enableConsoleLogging ?? true;

  /** Whether to forward log entries to the backend. */
  private remoteEnabled: boolean = environment.enableRemoteLogging ?? true;

  /** Flush interval in ms. */
  private readonly FLUSH_INTERVAL = 5_000;

  /** Maximum buffer size before an immediate flush. */
  private readonly MAX_BUFFER_SIZE = 25;

  // ── Public API ──────────────────────────────────────────────

  trace(source: string, message: string, ...data: unknown[]): void {
    this.log(LogLevel.Trace, source, message, data);
  }

  debug(source: string, message: string, ...data: unknown[]): void {
    this.log(LogLevel.Debug, source, message, data);
  }

  info(source: string, message: string, ...data: unknown[]): void {
    this.log(LogLevel.Info, source, message, data);
  }

  warn(source: string, message: string, ...data: unknown[]): void {
    this.log(LogLevel.Warn, source, message, data);
  }

  error(source: string, message: string, error?: unknown): void {
    const stack = error instanceof Error ? error.stack : undefined;
    const extra = error instanceof Error ? error.message : error ? String(error) : undefined;
    const fullMessage = extra ? `${message} — ${extra}` : message;
    this.log(LogLevel.Error, source, fullMessage, [], stack);
  }

  fatal(source: string, message: string, error?: unknown): void {
    const stack = error instanceof Error ? error.stack : undefined;
    const extra = error instanceof Error ? error.message : error ? String(error) : undefined;
    const fullMessage = extra ? `${message} — ${extra}` : message;
    this.log(LogLevel.Fatal, source, fullMessage, [], stack);
  }

  // ── Internal ────────────────────────────────────────────────

  private log(level: LogLevel, source: string, message: string, data: unknown[], stackTrace?: string): void {
    if (level < this.minLevel) return;

    const entry: LogEntry = {
      level: LogLevel[level].toLowerCase(),
      message,
      source,
      timestamp: new Date().toISOString(),
      stackTrace
    };

    // Console output
    if (this.consoleEnabled) {
      this.writeToConsole(level, source, message, data, stackTrace);
    }

    // Buffer for backend
    if (this.remoteEnabled && level >= LogLevel.Info) {
      this.buffer.push(entry);
      this.scheduleFlush();
    }
  }

  private writeToConsole(level: LogLevel, source: string, message: string, data: unknown[], stackTrace?: string): void {
    const timestamp = new Date().toLocaleTimeString();
    const prefix = `[${timestamp}] [${LogLevel[level].toUpperCase()}] [${source}]`;

    switch (level) {
      case LogLevel.Trace:
        console.debug(`%c${prefix}`, 'color: #888', message, ...data);
        break;
      case LogLevel.Debug:
        console.debug(`%c${prefix}`, 'color: #6c757d', message, ...data);
        break;
      case LogLevel.Info:
        console.info(`%c${prefix}`, 'color: #0d6efd', message, ...data);
        break;
      case LogLevel.Warn:
        console.warn(`${prefix}`, message, ...data);
        break;
      case LogLevel.Error:
        console.error(`${prefix}`, message, ...data);
        if (stackTrace) console.error(stackTrace);
        break;
      case LogLevel.Fatal:
        console.error(`%c${prefix} FATAL`, 'color: #fff; background: #dc3545; padding: 2px 6px; border-radius: 3px', message, ...data);
        if (stackTrace) console.error(stackTrace);
        break;
    }
  }

  private scheduleFlush(): void {
    if (this.buffer.length >= this.MAX_BUFFER_SIZE) {
      this.flush();
      return;
    }

    if (!this.flushTimer) {
      this.flushTimer = setTimeout(() => this.flush(), this.FLUSH_INTERVAL);
    }
  }

  private flush(): void {
    if (this.flushTimer) {
      clearTimeout(this.flushTimer);
      this.flushTimer = null;
    }

    if (this.buffer.length === 0) return;

    const entries = [...this.buffer];
    this.buffer = [];

    this.http
      .post(`${environment.apiURL}/api/clientlogs`, entries)
      .subscribe({
        error: (err) => {
          // If remote logging fails, write to console but don't re-buffer (avoid infinite loop)
          if (this.consoleEnabled) {
            console.warn('[LoggingService] Failed to send logs to backend:', err);
          }
        }
      });
  }
}
