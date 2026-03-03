# Logging Strategy — Serilog + Angular LoggingService

> Full-stack structured logging implementation for the Veggie World application.

---

## Table of Contents

1. [Overview](#overview)
2. [Backend — Serilog](#backend--serilog)
3. [Frontend — Angular LoggingService](#frontend--angular-loggingservice)
4. [Client-to-Server Log Forwarding](#client-to-server-log-forwarding)
5. [Log Levels Reference](#log-levels-reference)
6. [File Layout](#file-layout)
7. [Configuration](#configuration)
8. [Testing](#testing)
9. [Usage Examples](#usage-examples)

---

## Overview

| Concern | Technology | Key Feature |
|---------|-----------|-------------|
| Backend logging | **Serilog** (6 NuGet packages) | Structured parameters, dual sinks, enrichers |
| Frontend logging | **LoggingService** (custom Angular service) | Configurable levels, batched HTTP forwarding |
| Error handling | **GlobalErrorHandler** (Angular `ErrorHandler`) | Catches all unhandled exceptions |
| HTTP logging | **LoggingInterceptor** (Angular HTTP interceptor) | Request/response timing and error classification |
| Bridge | **ClientLogsController** (.NET controller) | Receives frontend logs and routes to Serilog |

---

## Backend — Serilog

### Packages Installed

| Package | Version | Purpose |
|---------|---------|---------|
| `Serilog.AspNetCore` | 8.0.3 | ASP.NET Core integration and request logging middleware |
| `Serilog.Sinks.Console` | 6.0.0 | Coloured, structured console output |
| `Serilog.Sinks.File` | 6.0.0 | Daily rolling file output |
| `Serilog.Enrichers.Environment` | 3.0.1 | Machine name and environment enrichment |
| `Serilog.Enrichers.Thread` | 4.0.0 | Thread ID enrichment |
| `Serilog.Expressions` | 5.0.0 | Expression-based filtering (e.g., error-only sink) |

### Bootstrap Configuration (`Program.cs`)

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/vegworld-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/vegworld-errors-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 90,
        restrictedToMinimumLevel: LogEventLevel.Error,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();
```

### Request Logging Middleware

Every HTTP request is automatically logged by `UseSerilogRequestLogging()` with:

- **Custom template:** `"HTTP {RequestMethod} {RequestPath} — {StatusCode} in {Elapsed:0.0000}ms"`
- **Response classification:**
  - 5xx → `LogEventLevel.Error`
  - 4xx → `LogEventLevel.Warning`
  - Elapsed > 5000ms → `LogEventLevel.Warning` (slow request detection)
- **Diagnostic context enrichment:**
  - `RequestHost` — origin hostname
  - `UserAgent` — browser/client identifier
  - `ClientIP` — client IP address
  - `UserName` — authenticated user (from JWT claims)

### Controller Logging Pattern

All 7 controllers inject `ILogger<T>` and follow these conventions:

```csharp
// ✅ CORRECT — Structured parameters
_logger.LogInformation("Retrieved {Count} categories", categories.Count);
_logger.LogWarning("Category with ID {CategoryId} not found", id);
_logger.LogError(ex, "Failed to create category with name {CategoryName}", dto.Name);

// ❌ WRONG — String interpolation (anti-pattern)
_logger.LogInformation($"Retrieved {categories.Count} categories");
```

**Controllers logging:**
- `VegCategoriesController` — All CRUD endpoints with `{CategoryId}`, `{CategoryName}`, `{Count}`
- `VegProductsController` — All CRUD endpoints including `GetProductsByCategory`
- `VegTypeWeightsController` — All CRUD endpoints
- `ProductImagesController` — Upload, update, delete operations with `{ProductId}`, `{ImageId}`
- `WeatherForecastController` — Request logging
- `AuthController` — Login/register events (pre-existing)
- `ClientLogsController` — Frontend log receipt

### Log File Locations

```
DotNetCoreWebApi/DotNetCoreWebApi/logs/
├── vegworld-20260303.log          # All logs (Info+), daily rotation, 30-day retention
└── vegworld-errors-20260303.log   # Error & Fatal only, daily rotation, 90-day retention
```

---

## Frontend — Angular LoggingService

### Architecture

```
core/
├── services/
│   └── logging.service.ts         # Centralized logging service
├── handlers/
│   └── global-error.handler.ts    # Global ErrorHandler → logger.fatal()
└── interceptors/
    └── logging.interceptor.ts     # HTTP request/response logging
```

### LoggingService (`core/services/logging.service.ts`)

A `providedIn: 'root'` service that provides:

- **Six log-level methods:** `trace()`, `debug()`, `info()`, `warn()`, `error()`, `fatal()`
- **Source tracking:** Every log call requires a source string (component/service name)
- **Configurable minimum level:** Via `environment.logLevel` (0=Trace through 6=Off)
- **Console output:** Colour-coded by level, toggleable via `environment.enableConsoleLogging`
- **Remote forwarding:** Batched HTTP POST to `/api/clientlogs`, toggleable via `environment.enableRemoteLogging`

#### Batching Strategy

- Logs are buffered in memory
- Flushed every **5 seconds** or when buffer reaches **25 entries** (whichever comes first)
- Only logs at `Info` level and above are forwarded to the backend
- Failed remote sends log a console warning but do not re-buffer (prevents infinite loops)

### GlobalErrorHandler (`core/handlers/global-error.handler.ts`)

Replaces Angular's default `ErrorHandler`:

```typescript
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private logger = inject(LoggingService);

  handleError(error: unknown): void {
    const err = error instanceof Error ? error : new Error(String(error));
    this.logger.fatal('GlobalErrorHandler', 'Unhandled error caught', err);
  }
}
```

Registered in `app.config.ts`:
```typescript
{ provide: ErrorHandler, useClass: GlobalErrorHandler }
```

### LoggingInterceptor (`core/interceptors/logging.interceptor.ts`)

A functional HTTP interceptor that:

| Event | Log Level | Details |
|-------|-----------|---------|
| Outgoing request | Debug | Method, URL |
| Successful response (2xx/3xx) | Debug | Method, URL, status, elapsed ms |
| Client error (4xx) | Warn | Method, URL, status, elapsed ms |
| Server error (5xx) | Error | Method, URL, status, elapsed ms |
| Network/unknown error | Error | Method, URL, error message |

**Self-referral prevention:** Requests to `/api/clientlogs` are passed through without logging to avoid infinite recursion.

Registered in `app.config.ts`:
```typescript
provideHttpClient(withInterceptors([jwtInterceptor, loggingInterceptor]))
```

---

## Client-to-Server Log Forwarding

### ClientLogsController (`Controllers/ClientLogsController.cs`)

```
POST /api/clientlogs
Content-Type: application/json

[
  {
    "level": "error",
    "message": "Failed to load product details — 404 Not Found",
    "source": "ProductDetailComponent",
    "timestamp": "2026-03-03T15:30:00.000Z",
    "stackTrace": "Error: ...\n    at ..."
  }
]
```

**Level routing:**

| Client Level | Serilog Method | Tag |
|-------------|----------------|-----|
| `error`, `fatal` | `LogError` | `[CLIENT]` |
| `warn`, `warning` | `LogWarning` | `[CLIENT]` |
| `debug`, `trace` | `LogDebug` | `[CLIENT]` |
| Other (e.g., `info`) | `LogInformation` | `[CLIENT]` |

All client-originated log entries are prefixed with `[CLIENT]` in the backend log output for easy filtering.

---

## Log Levels Reference

| Level | Numeric | Backend (Serilog) | Frontend (LoggingService) | Console Method | When to Use |
|-------|---------|-------------------|---------------------------|----------------|-------------|
| Trace | 0 | `Verbose` | `logger.trace()` | `console.debug` | Extremely detailed diagnostic data (not in production) |
| Debug | 1 | `Debug` | `logger.debug()` | `console.debug` | Development-time diagnostics, internal state |
| Info | 2 | `Information` | `logger.info()` | `console.info` | Normal operational events, user actions |
| Warn | 3 | `Warning` | `logger.warn()` | `console.warn` | Unexpected but recoverable situations |
| Error | 4 | `Error` | `logger.error()` | `console.error` | Failures that need attention, with stack traces |
| Fatal | 5 | `Fatal` | `logger.fatal()` | `console.error` | Application-stopping errors, unhandled exceptions |
| Off | 6 | — | — | — | Suppress all logging |

### Default Levels by Environment

| Environment | Backend Min Level | Frontend Min Level | Console Enabled | Remote Enabled |
|-------------|-------------------|--------------------|-----------------|----------------|
| **Development** | Debug | Trace (0) | ✅ Yes | ✅ Yes |
| **Production** | Information | Info (2) | ❌ No | ✅ Yes |

---

## File Layout

### Backend Files Modified/Created

| File | Change |
|------|--------|
| `Program.cs` | Serilog bootstrap, `UseSerilog()`, `UseSerilogRequestLogging()`, try/catch/finally |
| `appsettings.json` | Replaced `Logging` with `Serilog.MinimumLevel` configuration |
| `appsettings.Development.json` | Added `Serilog.MinimumLevel` (Default: Debug) |
| `Controllers/VegCategoriesController.cs` | Added `ILogger<T>`, structured logging across all endpoints |
| `Controllers/VegProductsController.cs` | Added `ILogger<T>`, structured logging across all endpoints |
| `Controllers/VegTypeWeightsController.cs` | Added `ILogger<T>`, structured logging across all endpoints |
| `Controllers/ProductImagesController.cs` | Fixed anti-patterns (string interpolation → structured), added missing logging |
| `Controllers/WeatherForecastController.cs` | Added debug log call (logger was injected but unused) |
| `Controllers/ClientLogsController.cs` | **NEW** — Receives frontend logs |
| `DotNetCoreWebApi.csproj` | Added 6 Serilog NuGet package references |

### Frontend Files Created

| File | Purpose |
|------|---------|
| `src/app/core/services/logging.service.ts` | Centralized logging service |
| `src/app/core/handlers/global-error.handler.ts` | Global error handler |
| `src/app/core/interceptors/logging.interceptor.ts` | HTTP interceptor for request/response logging |

### Frontend Files Modified

| File | Change |
|------|--------|
| `src/environments/environment.ts` | Added `logLevel`, `enableConsoleLogging`, `enableRemoteLogging` |
| `src/environments/environment.development.ts` | Added `logLevel`, `enableConsoleLogging`, `enableRemoteLogging` |
| `src/app/app.config.ts` | Registered `GlobalErrorHandler` and `loggingInterceptor` |

### Test Files

| File | Tests |
|------|-------|
| `DotNetCoreWebApi.Tests/Unit/ClientLogsControllerTests.cs` | 8 tests (valid entries, null/empty, all levels, stack traces) |
| `src/app/core/services/logging.service.spec.ts` | 8 tests (all log levels, Error objects, stack traces) |
| `src/app/core/interceptors/logging.interceptor.spec.ts` | 5 tests (request/response logging, 4xx/5xx, clientlogs skip) |
| `src/app/core/handlers/global-error.handler.spec.ts` | 5 tests (Error objects, non-Error wrapping, null/undefined) |

---

## Configuration

### Backend — `appsettings.json`

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

### Backend — `appsettings.Development.json`

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

### Frontend — `environment.ts` (Production)

```typescript
export const environment = {
  production: true,
  apiURL: 'https://localhost:7020',
  logLevel: 2,                  // Info and above
  enableConsoleLogging: false,   // No console output in production
  enableRemoteLogging: true,     // Forward to backend
};
```

### Frontend — `environment.development.ts`

```typescript
export const environment = {
  production: false,
  apiURL: 'https://localhost:7020',
  logLevel: 0,                  // Trace and above (show everything)
  enableConsoleLogging: true,    // Full console output
  enableRemoteLogging: true,     // Forward to backend
};
```

---

## Testing

### Backend Tests

All 179 backend tests pass, including 12 new tests for the `ClientLogsController`:

```powershell
cd DotNetCoreWebApi
dotnet test
# Result: 179 passed, 0 failed
```

### Frontend Tests

18 new tests across 3 spec files — all pass:

```powershell
cd angular-app
npx ng test --no-watch
```

| Test File | Tests | Status |
|-----------|-------|--------|
| `logging.service.spec.ts` | 8 | ✅ Pass |
| `logging.interceptor.spec.ts` | 5 | ✅ Pass |
| `global-error.handler.spec.ts` | 5 | ✅ Pass |

---

## Usage Examples

### Backend — Controller Logging

```csharp
public class VegCategoriesController : ControllerBase
{
    private readonly ILogger<VegCategoriesController> _logger;

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        _logger.LogInformation("Retrieving all categories");
        var categories = await _categoryService.GetAllAsync();
        _logger.LogInformation("Retrieved {Count} categories", categories.Count);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult> Create(VegCategoryDto dto)
    {
        _logger.LogInformation("Creating category with name {CategoryName}", dto.Name);
        try
        {
            var result = await _categoryService.CreateAsync(dto);
            _logger.LogInformation("Created category {CategoryId}: {CategoryName}", result.Id, result.Name);
            return CreatedAtAction(...);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create category with name {CategoryName}", dto.Name);
            return StatusCode(500, "An error occurred");
        }
    }
}
```

### Frontend — Component Logging

```typescript
export class ProductListComponent implements OnInit {
  private logger = inject(LoggingService);

  ngOnInit() {
    this.logger.info('ProductList', 'Component initialized');
    this.loadProducts();
  }

  loadProducts() {
    this.logger.debug('ProductList', 'Loading products from API');
    this.productService.getAll().subscribe({
      next: (products) => {
        this.logger.info('ProductList', `Loaded ${products.length} products`);
        this.products = products;
      },
      error: (err) => {
        this.logger.error('ProductList', 'Failed to load products', err);
      }
    });
  }
}
```

### Frontend — Service Logging

```typescript
export class AuthService {
  private logger = inject(LoggingService);

  login(credentials: LoginDto) {
    this.logger.info('AuthService', 'Login attempt', credentials.email);
    return this.http.post('/api/auth/login', credentials).pipe(
      tap(() => this.logger.info('AuthService', 'Login successful')),
      catchError(err => {
        this.logger.error('AuthService', 'Login failed', err);
        return throwError(() => err);
      })
    );
  }
}
```
