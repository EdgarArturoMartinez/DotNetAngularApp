# Veggie World E-Commerce Platform

A full-stack e-commerce application for selling vegetables, built with .NET 8 and Angular 19.

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- SQL Server (LocalDB or full instance)
- Visual Studio Code or Visual Studio 2022

### Running the Application

#### 1. Start Backend API

```powershell
cd DotNetCoreWebApi/DotNetCoreWebApi
dotnet run
```

Backend will run on: `https://localhost:7020`

#### 2. Start Frontend (in new terminal)

```powershell
cd angular-app
npm start
```

Frontend will run on: `http://localhost:4200` (or `http://localhost:4201` if port 4200 is in use)

#### 3. Access Application

**Public eCommerce Site:** Open browser to `http://localhost:4200/`  
**Admin Panel:** Navigate to `http://localhost:4200/admin`

---

## 📁 Project Structure

```
├── DotNetCoreWebApi/          # .NET 8 Web API
│   ├── Application/           # Business logic and entities
│   │   ├── Entities/          # Domain models (VegProducts, VegCategory, etc.)
│   │   ├── Interfaces/        # Service and repository interfaces
│   │   ├── Services/          # Business logic implementation
│   │   └── DBContext/         # Entity Framework DbContext
│   ├── Infrastructure/        # Data access repositories
│   ├── Controllers/           # API endpoints
│   ├── DTOs/                  # Data transfer objects
│   └── Migrations/            # EF Core database migrations
├── angular-app/               # Angular 19 frontend
│   ├── src/app/
│   │   ├── home/              # Public eCommerce landing page
│   │   ├── landing/           # Admin dashboard
│   │   ├── menu/              # Admin navigation menu
│   │   ├── index-products/    # Product management (admin)
│   │   ├── index-vegcategories/ # Category management (admin)
│   │   ├── create-vegproduct/ # Create product form (admin)
│   │   ├── edit-vegproduct/   # Edit product form (admin)
│   │   ├── create-vegcategory/ # Create category form (admin)
│   │   ├── edit-vegcategory/  # Edit category form (admin)
│   │   ├── core/             # Core infrastructure (logging, interceptors, error handling)
│   │   ├── features/          # Feature modules with services
│   │   ├── shared/            # Shared models, services, and components
│   │   │   ├── components/    # Reusable UI components
│   │   │   │   └── generic-data-table/ # Reusable data table with dual views
│   │   │   └── models/        # TypeScript interfaces
│   │   └── [other components]
└── docs/                      # Project documentation
    ├── architecture/          # Architecture guides and scenarios
    ├── implementation-guides/ # Step-by-step implementation guides
    └── completed-features/    # Completed feature summaries
```

### Routing Structure

The application uses a clear separation between public and admin routes:

**Public Routes (No authentication required):**
- `/` - eCommerce landing page with featured products and categories
- Future: `/shop`, `/products/:id`, `/cart`, `/checkout`

**Admin Routes (Admin panel for inventory management):**
- `/admin` - Dashboard with statistics and insights
- `/admin/products` - Product management (list, create, edit, delete)
- `/admin/categories` - Category management
- `/admin/reviews` - Review management (example from Scenario 2)

**Navigation Behavior:**
- Admin menu only appears on `/admin/*` routes
- Public pages have clean, distraction-free layout
- Old routes automatically redirect to new `/admin` prefixed routes

---

## 🏗️ Architecture

### Backend - Clean Architecture
- **Controllers:** Thin API layer
- **Services:** Business logic
- **Repositories:** Data access
- **Entities:** Domain models
- **DTOs:** API contracts

### Frontend - Feature-based
- **Features:** Organized by domain (categories, products)
- **Shared:** Reusable services, models, components
- **Standalone Components:** Using Angular 19 standalone API

### Technologies
- **.NET 8** - Backend API
- **Entity Framework Core** - ORM
- **Serilog** - Structured logging (Console + File sinks)
- **Angular 19** - Frontend framework
- **Angular Material** - UI components
- **SQL Server** - Database
- **RxJS** - Reactive programming

---

## 🎯 Common Development Scenarios

This project includes detailed step-by-step guides for common development tasks. See [ARCHITECTURE.md](docs/architecture/ARCHITECTURE.md) for complete instructions:

### Scenario 1: Adding a New Field to an Existing Table

**Use case:** You want to add a new property (e.g., `ImageUrl`) to an existing entity like `VegProducts`.

**What you'll learn:**
- Update entity class and DTOs
- Modify service layer mappings
- Create and apply database migration
- Update TypeScript interfaces
- Add form fields to create/edit components
- Display new field in data table

**📍 Location:** [ARCHITECTURE.md - Scenario 1](docs/architecture/ARCHITECTURE.md#scenario-1-adding-a-new-field-to-existing-entity-vegproducts)

---

### Scenario 2: Creating a New Entity with Foreign Key to VegProducts

**Use case:** You want to create a completely new entity (e.g., `ProductReview`) that references `VegProducts` with a foreign key relationship.

**What you'll learn:**
- Create complete entity structure (8 backend files)
- Configure foreign key relationships in DbContext
- Set up repository and service layers
- Create controller with all CRUD endpoints
- Register services in dependency injection
- Build complete Angular CRUD interface (index, create, edit components)
- Use generic-data-table component
- Add routes with `/admin` prefix
- Handle dropdown selection for foreign keys
- Test cascade delete behavior

**📍 Location:** [ARCHITECTURE.md - Scenario 2](docs/architecture/ARCHITECTURE.md#scenario-2-creating-new-entity-with-foreign-key-to-vegproducts)

---

## 📚 Documentation

All documentation has been organized into the `/docs` folder for better project structure:

### Architecture Documentation (`docs/architecture/`)
- **[ARCHITECTURE.md](docs/architecture/ARCHITECTURE.md)** - Complete architecture guide with detailed scenarios:
  - **Scenario 1:** Adding a new field to an existing entity (VegProducts)
  - **Scenario 2:** Creating a new entity with foreign key relationship to VegProducts
- **[GENERIC-DATA-TABLE-GUIDE.md](docs/architecture/GENERIC-DATA-TABLE-GUIDE.md)** - Reusable data table component
- **[QUICK-START-DATA-TABLE.md](docs/architecture/QUICK-START-DATA-TABLE.md)** - Quick start for data tables
- **[PRODUCT-IMAGES-ARCHITECTURE.md](docs/architecture/PRODUCT-IMAGES-ARCHITECTURE.md)** - Product image system architecture
- **[FILE-UPLOAD-ARCHITECTURE.md](docs/architecture/FILE-UPLOAD-ARCHITECTURE.md)** - File upload system design
- **[LOGGING-STRATEGY.md](docs/architecture/LOGGING-STRATEGY.md)** - Serilog logging strategy (backend + frontend)

### Implementation Guides (`docs/implementation-guides/`)
- **[FRONTEND-IMPLEMENTATION-GUIDE.md](docs/implementation-guides/FRONTEND-IMPLEMENTATION-GUIDE.md)** - Frontend development guide
- **[PRODUCT-IMAGES-GUIDE.md](docs/implementation-guides/PRODUCT-IMAGES-GUIDE.md)** - Product images implementation

### Completed Features (`docs/completed-features/`)
- **[DATA-TABLE-IMPLEMENTATION-SUMMARY.md](docs/completed-features/DATA-TABLE-IMPLEMENTATION-SUMMARY.md)** - Data table feature summary
- **[IMPLEMENTATION-COMPLETE.md](docs/completed-features/IMPLEMENTATION-COMPLETE.md)** - General implementation status
- **[REFACTORING-SUMMARY.md](docs/completed-features/REFACTORING-SUMMARY.md)** - Clean architecture refactoring
- **[FILE-UPLOAD-IMPLEMENTATION-COMPLETE.md](docs/completed-features/FILE-UPLOAD-IMPLEMENTATION-COMPLETE.md)** - File upload completion
- **[VEGTYPEWEIGHT-IMPLEMENTATION-COMPLETE.md](docs/completed-features/VEGTYPEWEIGHT-IMPLEMENTATION-COMPLETE.md)** - VegTypeWeight feature
- **[MODAL-NOTIFICATION-SYSTEM.md](docs/completed-features/MODAL-NOTIFICATION-SYSTEM.md)** - Shared modal & notification system
- **[ADMIN-FEATURES-IMPLEMENTATION.md](docs/completed-features/ADMIN-FEATURES-IMPLEMENTATION.md)** - Admin features implementation
- **[ADMIN-NAVIGATION-REFACTORING.md](docs/completed-features/ADMIN-NAVIGATION-REFACTORING.md)** - Admin navigation refactoring
- **[BACKEND-TESTING-IMPLEMENTATION-COMPLETE.md](docs/completed-features/BACKEND-TESTING-IMPLEMENTATION-COMPLETE.md)** - Backend testing implementation

### Implementation Guides (`docs/implementation-guides/`) - continued
- **[FRONTEND-AUTH-GUIDE.md](docs/implementation-guides/FRONTEND-AUTH-GUIDE.md)** - Frontend authentication guide
- **[FRONTEND-AUTH-QUICK-START.md](docs/implementation-guides/FRONTEND-AUTH-QUICK-START.md)** - Frontend auth quick start

### Project Assessment
- **[PROJECT-ASSESSMENT-AND-RECOMMENDATIONS.md](docs/PROJECT-ASSESSMENT-AND-RECOMMENDATIONS.md)** - Project assessment and recommendations

### Angular Documentation
- **[angular-app/README.md](angular-app/README.md)** - Angular-specific documentation

---

## 🔧 Development

### Database Migrations

```powershell
cd DotNetCoreWebApi/DotNetCoreWebApi

# Create migration
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update
```

### Code Organization

#### Backend Checklist
- ✅ Entity in `Application/Entities`
- ✅ DTOs in `DTOs` folder
- ✅ Repository interface and implementation
- ✅ Service interface and implementation
- ✅ Controller with thin endpoints
- ✅ Register dependencies in `Program.cs`

#### Frontend Checklist
- ✅ Interface in `shared/models/entities.ts`
- ✅ Service in `features/{feature}/services`
- ✅ Index, Create, Edit components
- ✅ Routes in `app.routes.ts`

---

## 🎯 Features

### Current Entities
- **Categories:** Product categories with relationships
- **Products:** Products with pricing, stock, and category assignment

### Implemented Functionality
- ✅ CRUD operations for all entities
- ✅ Foreign key relationships
- ✅ Validation (frontend and backend)
- ✅ Error handling and notifications
- ✅ Loading states
- ✅ Confirmation dialogs
- ✅ Responsive UI
- ✅ Structured logging with Serilog (backend) and LoggingService (frontend)
- ✅ Global error handling
- ✅ HTTP request/response logging

---

## 📊 Logging

The application uses a production-grade structured logging strategy across the full stack.

### Backend (Serilog)
- **Sinks:** Console (coloured, structured) + Rolling File (daily rotation, 30-day retention)
- **Error Log:** Separate error-only file with 90-day retention
- **Enrichers:** Machine name, environment, thread ID
- **Request Logging:** Every HTTP request logged with method, path, status, and elapsed time
- **Diagnostic Context:** Client IP, User-Agent, authenticated user
- **Structured Parameters:** All controllers use `{Placeholder}` syntax — no string interpolation
- **Configuration:** Minimum levels set in `appsettings.json` with per-namespace overrides

### Frontend (LoggingService)
- **Centralized service** at `core/services/logging.service.ts` with `trace`, `debug`, `info`, `warn`, `error`, `fatal` methods
- **Configurable** minimum level and console/remote toggles via `environment.ts`
- **Batched forwarding** to backend `/api/clientlogs` endpoint (flush every 5s or at 25 entries)
- **Global error handler** catches all unhandled exceptions
- **HTTP interceptor** logs outgoing requests, responses, and errors with timing

### Log Levels
| Level | Backend | Frontend | Usage |
|-------|---------|----------|-------|
| Trace | Verbose | LogLevel.Trace | Detailed diagnostic data |
| Debug | Debug | LogLevel.Debug | Development diagnostics |
| Info | Information | LogLevel.Info | Normal operational events |
| Warn | Warning | LogLevel.Warn | Unexpected but recoverable |
| Error | Error | LogLevel.Error | Failures requiring attention |
| Fatal | Fatal | LogLevel.Fatal | Application-stopping errors |

**📍 Full details:** [LOGGING-STRATEGY.md](docs/architecture/LOGGING-STRATEGY.md)

---

## 🌐 API Endpoints

### Products
- `GET /api/vegproducts` - Get all products
- `GET /api/vegproducts/{id}` - Get product by ID
- `POST /api/vegproducts` - Create product
- `PUT /api/vegproducts/{id}` - Update product
- `DELETE /api/vegproducts/{id}` - Delete product
- `GET /api/vegproducts/category/{categoryId}` - Get products by category

### Categories  
- `GET /api/vegcategories` - Get all categories
- `GET /api/vegcategories/{id}` - Get category by ID
- `POST /api/vegcategories` - Create category
- `PUT /api/vegcategories/{id}` - Update category
- `DELETE /api/vegcategories/{id}` - Delete category

---

## 🧪 Testing

### Backend Tests
```powershell
cd DotNetCoreWebApi/DotNetCoreWebApi
dotnet test
```

### Frontend Tests
```powershell
cd angular-app
npm test
```

---

## 📝 Configuration

### Backend - appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DBVegProducts;..."
  },
  "CorsOrigins": ["http://localhost:4200", "https://localhost:4200"]
}
```

### Frontend - environment.development.ts
```typescript
export const environment = {
  production: false,
  apiURL: 'https://localhost:7020'
};
```

---

## 🚨 Troubleshooting

### Backend Not Starting
- Check SQL Server is running
- Verify connection string in `appsettings.Development.json`
- Run `dotnet ef database update`

### CORS Errors
- Verify backend CorsOrigins includes frontend URL
- Check backend is running on https://localhost:7020
- Frontend should use https://localhost:7020 (not http)

### Angular Errors
- Run `npm install` if dependencies are missing
- Clear `.angular/cache` folder
- Check Node.js version (18+)

---

## 📦 Build for Production

### Backend
```powershell
dotnet publish -c Release -o ./publish
```

### Frontend
```powershell
cd angular-app
npm run build
```

Production files will be in `angular-app/dist/`

---

## 👥 Contributing

1. Create feature branch
2. Follow existing code patterns
3. Update tests
4. Update documentation
5. Submit pull request

---

## 📄 License

This project is for educational purposes.

---

## 🎓 Learning Resources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Angular Best Practices](https://angular.dev/style-guide)

---

## ⭐ Version History

- **v1.0** - Initial release with Categories and Products
- Refactored to Clean Architecture (Feb 2026)
- Implemented Repository and Service patterns
- Organized Angular with feature-based structure
