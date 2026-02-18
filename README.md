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

Frontend will run on: `http://localhost:4200`

#### 3. Access Application

Open browser to http://localhost:4200

---

## 📁 Project Structure

```
├── DotNetCoreWebApi/          # .NET 8 Web API
│   ├── Application/           # Business logic and entities
│   ├── Infrastructure/        # Data access repositories
│   ├── Controllers/           # API endpoints
│   └── DTOs/                  # Data transfer objects
├── angular-app/               # Angular 19 frontend
│   ├── src/app/
│   │   ├── features/          # Feature modules with services
│   │   ├── shared/            # Shared models and services
│   │   └── [components]/      # UI components
└── ARCHITECTURE.md            # Complete architecture documentation
```

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
- **Angular 19** - Frontend framework
- **Angular Material** - UI components
- **SQL Server** - Database
- **RxJS** - Reactive programming

---

## 📚 Documentation

- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Complete architecture guide and adding new entities
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
