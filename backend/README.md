# BallastLane Product Management API

Backend built with **.NET 8** following **Clean Architecture** principles (Domain, Application, Infrastructure, Presentation).

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | NHibernate + FluentNHibernate |
| Database | Microsoft SQL Server |
| Authentication | JWT Bearer tokens |
| Password hashing | BCrypt.Net |
| Logging | Serilog (Console + File) |
| API Docs | Swagger / OpenAPI |
| Testing | xUnit + Moq + FluentAssertions |

## Clean Architecture Layers

```
backend/
├── src/
│   ├── BallastLane.ProductManagement.Domain/            # Entities + Repository interfaces (no dependencies)
│   │   ├── Entities/       Product.cs, User.cs
│   │   └── Interfaces/     IProductRepository, IUserRepository
│   │
│   ├── BallastLane.ProductManagement.Application/       # DTOs, Service interfaces, Service implementations
│   │   ├── Dtos/           ProductDto, CreateProductDto, UpdateProductDto, LoginDto, RegisterUserDto, AuthResponseDto
│   │   ├── Interfaces/     IProductService, IAuthService
│   │   └── Services/       ProductService, AuthService (BCrypt + JWT)
│   │
│   ├── BallastLane.ProductManagement.Infrastructure/    # Persistence details (NHibernate)
│   │   ├── Mappings/       ProductMap, UserMap (FluentNHibernate)
│   │   └── Repositories/   ProductRepository, UserRepository
│   │
│   ├── BallastLane.ProductManagement.API/               # Presentation layer (Controllers, DI, Middleware)
│   │   ├── Controllers/    ProductsController, AuthController
│   │   └── Program.cs
│   │
│   └── BallastLane.ProductManagement.Tests/             # Unit tests
│       └── Unit/           ProductsControllerTests, ProductServiceTests, AuthServiceTests
│
├── db/
│   └── init.sql                                         # SQL Server schema
└── Dockerfile
```

### Dependency Flow

```
Domain ← Application ← Infrastructure
                     ← API (Presentation)
                     ← Tests
```

- **Domain** has zero external dependencies (pure .NET)
- **Application** depends only on Domain
- **Infrastructure** depends only on Domain (implements its interfaces)
- **API** references Domain, Application, and Infrastructure for DI wiring

## Endpoints

### Auth
| Method | Route | Description |
|---|---|---|
| POST | `/api/v1/auth/register` | Register a new user |
| POST | `/api/v1/auth/login` | Login and get JWT token |

### Products (requires Bearer token)
| Method | Route | Description |
|---|---|---|
| GET | `/api/v1/products` | Get all products |
| GET | `/api/v1/products/{id}` | Get product by Id |
| POST | `/api/v1/products` | Create a product |
| PUT | `/api/v1/products/{id}` | Update a product |
| DELETE | `/api/v1/products/{id}` | Delete a product |

## Run Locally

### Prerequisites
- .NET 8 SDK
- SQL Server (local or Docker)

### 1. Database
Run `db/init.sql` against your SQL Server instance to create the `BallastLaneDb` database and tables.

### 2. Configuration
Update `appsettings.Development.json` with your local SQL Server connection string.

### 3. Run
```bash
cd src/BallastLane.ProductManagement.API
dotnet run
```

Swagger UI will be available at: `http://localhost:{port}/swagger`

## Run Tests

```bash
dotnet test src/BallastLane.ProductManagement.sln
```

## Run with Docker

```bash
docker build -t ballastlane-api .
docker run -p 8080:8080 ballastlane-api
```

> See root `docker-compose.yml` to run the full stack (API + SQL Server + Angular).
