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
├── backend/                # .NET 8 Web API (Clean Architecture)
│   ├── src/
│   ├── db/
│   └── Dockerfile
├── frontend/               # Angular application
│   └── Dockerfile
├── Working App Screenshots/# Screenshots of the fully functional application
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

## How to Access & Verify the Demo

Once the application is running (either via Docker Compose or manually):

1. **Open the Frontend**: Go to **`http://localhost:4200`** in your browser.
2. **Register a User**: You will be redirected to the Login page. Click on **Register** (or go directly to `http://localhost:4200/register`).
3. **Fill the Form**: Provide a username, email, and password (minimum 6 characters).
4. **Explore the Dashboard**: Upon registering, you are logged in automatically and redirected to the **Products Dashboard** (`http://localhost:4200/products`).
5. **View Seed Data**: You will instantly see the **5 default products** pre-loaded from the database seed script:
   - *Wireless Mechanical Keyboard*
   - *Ergonomic Wireless Mouse*
   - *UltraWide Curved Monitor*
   - *Noise-Cancelling Headphones*
   - *USB-C Dual HDMI Docking Station*
6. **Test Features**:
   - Use the Material **paginator** at the bottom of the table to navigate pages.
   - Perform full **CRUD** operations by creating, editing, and deleting products.

## Working App Screenshots

You can find screenshots of the fully functional application in the **`Working App Screenshots/`** directory. These images showcase:
- **User Authentication**: Secure Login and Registration forms with active, real-time input validations.
- **Products Dashboard**: Minimalist Material table presenting the 5 seeded products.
- **Paging & Navigation**: Fully functional pagination controls working directly with the backend database offsets.
- **Product Operations**: Interactive modals for creating, updating, and deleting products.

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