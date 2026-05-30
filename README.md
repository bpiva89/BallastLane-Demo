# BallastLane - Product Management Demo

Full-stack application built as part of the BallastLane technical interview exercise.

## Stack

| Layer | Technology |
|---|---|
| Backend | .NET 8, ASP.NET Core Web API, Clean Architecture |
| ORM | NHibernate + FluentNHibernate |
| Database | Microsoft SQL Server |
| Auth | JWT Bearer (BCrypt password hashing) |
| Logging | Serilog |
| Frontend | Angular (see `frontend/` folder) |
| Containerization | Docker + Docker Compose |

## Design Philosophy

The frontend follows a **minimalist design** intentionally — the goal of this exercise is to demonstrate backend architecture, API design, and Clean Architecture patterns, not UI/UX polish. Styles are kept simple and functional.

## Repository Structure

```
BallastLane-Demo/
├── backend/         # .NET 8 Web API (Clean Architecture)
│   ├── src/
│   ├── db/
│   └── Dockerfile
├── frontend/        # Angular application
│   └── Dockerfile
├── docker-compose.yml
└── README.md
```

## Running the Full Stack with Docker Compose

### Prerequisites
- Docker Desktop installed and running

### Steps

```bash
docker-compose up --build
```

This will spin up:
- **mssql** — SQL Server 2022 on port `1433`
- **api** — .NET 8 API on port `8080`
- **frontend** — Angular app on port `4200`

Services:
- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- Frontend: `http://localhost:4200`

### Tear down

```bash
docker-compose down -v
```

---

## Running Manually (without Docker)

### 1. Start SQL Server

You need a SQL Server instance. Run `backend/db/init.sql` to create the database and tables.

### 2. Backend

```bash
cd backend/src/BallastLane.ProductManagement.API
dotnet run
```

Update `appsettings.Development.json` with your SQL Server connection string before running.

### 3. Frontend

```bash
cd frontend
npm install
ng serve
```

---

## Running Tests

```bash
cd backend
dotnet test src/BallastLane.ProductManagement.sln
```

---

## Part 2: Generative AI Tools

The required documentation and workflow review for **Part 2: Generative AI Tools** can be found in the root of this workspace:
📄 **[Part 2 - Generative AI Tools.md](Part%202%20-%20Generative%20AI%20Tools.md)**

This document details:
- **Prompt Engineering**: The precise, structure-driven prompts used to scaffold clean, domain-driven architectures.
- **Architectural Control & Validation**: Iterative adjustments (such as maintaining absolute POCO purity in Domain entities by disabling NHibernate lazy-load proxies via mapping).
- **Edge-Case Resolution**: Resolving cross-platform/Linux container SQL Server driver incompatibilities (`MicrosoftDataSqlClientDriver` migration).

---

## API Overview

See `backend/README.md` for full endpoint documentation.

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/v1/auth/register` | No | Register user |
| POST | `/api/v1/auth/login` | No | Login, get JWT |
| GET | `/api/v1/products` | Yes | List all products |
| GET | `/api/v1/products/{id}` | Yes | Get product by Id |
| POST | `/api/v1/products` | Yes | Create product |
| PUT | `/api/v1/products/{id}` | Yes | Update product |
| DELETE | `/api/v1/products/{id}` | Yes | Delete product |