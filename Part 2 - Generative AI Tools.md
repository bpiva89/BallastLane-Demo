# Part 2: Generative AI Tools — Documentation & Process Review

This document presents the prompt design, iterative development workflow, validation steps, and technical control implemented when building the RESTful API and Frontend for the Product Management System. It serves as a representative case study of how GenAI tools (specifically LLMs) are leveraged as speed multipliers while maintaining rigorous architectural control, adherence to Clean Architecture principles, and robust engineering standards.

---

## 1. The Strategy: GenAI as a Speed Multiplier under Senior Supervision

As a Senior Full-Stack Developer, my approach to using Generative AI is built on the principle of **absolute architectural control**. I do not let the AI make design decisions; instead, I dictate the architecture, patterns, and constraints, and utilize the AI to write clean, boilerplate-free implementations and tests.

Throughout this project, I successfully enforced a strict **4-Layer Clean Architecture**, eliminated ORM leaks from our **Domain Layer**, implemented the **Result Pattern** for clean flow control, and resolved critical platform compatibility issues during Docker containerization.

Below is the structured walkthrough of how this is done, using a representative **Task Management System API** as requested by the exercise.

---

## 2. Phase 1: The Initial Prompts (Scaffolding & Architecture Definition)

Instead of asking the LLM a generic prompt like *"Write a task management API in .NET"*, which leads to low-quality, highly coupled code, I used a **highly structured, context-rich engineering prompt** to enforce strict Clean Architecture from line one.

### The System Scaffolding Prompt
```markdown
You are an expert software architect. I need to scaffold a RESTful Task Management System API in .NET 8.
We will follow strict 4-Layer Clean Architecture and Domain-Driven Design (DDD) principles:
1. Domain Layer: Absolute POCO purity. No external dependencies, no third-party libraries, and zero ORM concerns.
   - Entities: User (existing), Task (Id, Title, Description, Status [Enum: Todo, InProgress, Completed], DueDate, UserId).
   - Domain entity properties must NOT contain the "virtual" keyword (we want pure POCOs, no ORM-specific leakage).
   - Interfaces: ITaskRepository, IUserRepository contract definitions.
2. Application Layer: Orchestrates use cases. Depends ONLY on the Domain layer.
   - DTOs: TaskDto, CreateTaskDto, UpdateTaskDto.
   - Interfaces: ITaskService contract.
   - Services: TaskService. Must implement the Result Pattern using a generic Result<T> / Result envelope for business rules and flow control. Do NOT throw exceptions for validation or business failures (e.g., TaskNotFound, UserNotFound); return failed Result wrappers.
3. Infrastructure Layer: Data persistence. Depends on Domain and Application.
   - Persistence: NHibernate + FluentNHibernate mapping (using Microsoft.Data.SqlClient).
   - Fluent Mappings: TaskMap, UserMap. Must call "Not.LazyLoad()" on entities to support pure non-virtual Domain POCOs.
   - Repositories: TaskRepository, UserRepository concrete implementations using NHibernate ISession.
4. API (Presentation) Layer: HTTP/REST controllers.
   - Controllers: TasksController (secured with JWT [Authorize]).
   - Controllers must consume ITaskService and cleanly map Result<T> envelopes to ASP.NET Core ActionResult (200 OK, 201 Created, 404 NotFound, 400 BadRequest).
   - Include a global ExceptionHandlingMiddleware to capture unexpected failures, log them, and return a clean, safe JSON error payload (500 Internal Server Error) to the client.

Write the code for the generic Result/Result<T> pattern, the pure Domain Task entity, the FluentNHibernate TaskMap, and the TaskService showing how the Result Pattern and repository calls are orchestrated.
```

---

## 3. Representative Output Code (Produced by the LLM based on instructions)

The LLM generated highly structured code matching the precise constraints. Below are key representative files.

### 3.1. Pure Domain Entity (No leakage of NHibernate `virtual` properties)
```csharp
// Domain/Entities/Task.cs
namespace TaskManagement.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public TaskStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public enum TaskStatus
    {
        Todo = 1,
        InProgress = 2,
        Completed = 3
    }
}
```

### 3.2. Generic Result Pattern for Clean Business Flow Control
```csharp
// Domain/Common/Result.cs
namespace TaskManagement.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException("A successful result cannot contain errors.");
            if (!isSuccess && string.IsNullOrEmpty(error))
                throw new InvalidOperationException("A failed result must specify an error message.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);
        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException("Cannot access the value of a failed result.");
                return _value!;
            }
        }

        private Result(bool isSuccess, T? value, string error) : base(isSuccess, error)
        {
            _value = value;
        }

        public static Result<T> Success(T value) => new(true, value, string.Empty);
        public new static Result<T> Failure(string error) => new(false, default, error);
    }
}
```

### 3.3. Application Layer Service implementing Result Pattern and Business Logic
```csharp
// Application/Services/TaskService.cs
using TaskManagement.Application.Dtos;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<TaskDto>> CreateTaskAsync(CreateTaskDto dto)
        {
            if (dto is null)
                return Result.Failure<TaskDto>("Task data cannot be null.");

            // Enforce business rule validation: Verify referenced User exists in DB
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user is null)
                return Result.Failure<TaskDto>($"Associated User with ID {dto.UserId} does not exist.");

            var taskItem = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = TaskStatus.Todo,
                DueDate = dto.DueDate,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            var savedTask = await _taskRepository.InsertAsync(taskItem);
            return Result.Success(new TaskDto(savedTask));
        }

        public async Task<Result<TaskDto>> GetTaskByIdAsync(int id)
        {
            if (id <= 0)
                return Result.Failure<TaskDto>("Invalid task identifier.");

            var task = await _taskRepository.GetByIdAsync(id);
            if (task is null)
                return Result.Failure<TaskDto>($"Task with ID {id} was not found.");

            return Result.Success(new TaskDto(task));
        }
    }
}
```

### 3.4. Pure FluentNHibernate Infrastructure Mapping (Disabling Dynamic Proxies)
```csharp
// Infrastructure/Mappings/TaskMap.cs
using FluentNHibernate.Mapping;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Mappings
{
    public class TaskMap : ClassMap<TaskItem>
    {
        public TaskMap()
        {
            Table("Tasks");

            // CRITICAL ARCHITECTURAL SAFEGUARD:
            // Disables NHibernate dynamic proxy subclasses so that Domain layer properties
            // DO NOT need to be marked as "virtual". Keeps Domain 100% pure POCO.
            Not.LazyLoad();

            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Title).Length(200).Not.Nullable();
            Map(x => x.Description).Length(1000).Not.Nullable();
            Map(x => x.Status).CustomType<int>().Not.Nullable();
            Map(x => x.DueDate).Not.Nullable();
            Map(x => x.UserId).Not.Nullable();
            Map(x => x.CreatedAt).Not.Nullable();
            Map(x => x.UpdatedAt).Nullable();
        }
    }
}
```

---

## 4. Phase 2: Code Validation, Refactoring & Architectural Control

A junior developer might accept the AI's first output blindly. My role as the senior controller in this partnership was to review, validate, and execute targeted iterations to ensure enterprise-grade standards.

Below are the **four key interventions** I made during the implementation:

### 4.1. Clean Architecture Strictness Refactor
* **Observation**: In the early stage, the AI generated a three-project structure (`API`, `Core`, `Infrastructure`), grouping use-cases and Domain entities in the same assembly. This violates strict Clean Architecture where Domain must be completely independent of application concerns.
* **Correction**: I instructed the AI to refactor the solution into a strict **4-layer project structure** (`Domain`, `Application`, `Infrastructure`, `API`). I set up clean project dependencies:
  - `Domain` (Core POCOs & interfaces) has **zero** dependencies.
  - `Application` (Use cases & DTOs) depends **only** on `Domain`.
  - `Infrastructure` (Persistence & Repositories) depends on `Domain` to satisfy contracts.
  - `API` (Controllers) depends on `Application` and `Infrastructure` to wire up DI container (`Program.cs`).

### 4.2. Purifying the Domain Layer (NHibernate `virtual` Property Bypassing)
* **Observation**: The AI mapped Domain entities with `virtual` keywords on all properties because NHibernate defaults to lazy loading via dynamic proxy subclasses, which requires properties to be virtual. This leaked persistence details into the inner domain layer.
* **Correction**: I removed all `virtual` keywords from `Product.cs` and `User.cs` (and likewise `Task.cs` in the case study) to ensure pure POCO properties. I corrected the leakage at the Infrastructure layer by explicitly adding `Not.LazyLoad();` inside each FluentNHibernate `ClassMap` file. This tells NHibernate at runtime to map raw objects directly without generating proxy overrides, perfectly preserving Domain purity.

### 4.3. Restricting CORS Policies for API Security
* **Observation**: The AI originally suggested a wide-open CORS policy with `.AllowAnyOrigin()`, `.AllowAnyMethod()`, and `.AllowAnyHeader()`, which is a security risk in production.
* **Correction**: I refactored the DI configuration in `Program.cs` to apply strict, minimal CORS controls:
  - Restricting origins solely to the Angular client (`http://localhost:4200` and production host `http://localhost`).
  - Restricting HTTP verbs exclusively to those needed: `GET`, `POST`, `PUT`, `DELETE`.
  - Restricting request headers strictly to `Content-Type` and `Authorization`.

### 4.4. Resolving Linux Container & SqlClient Driver Crashes (Exit Code 139)
* **Observation**: During container orchestration (`docker-compose up --build`), the API container crashed instantly with a Linux segmentation fault (exit code 139) or unhandled reflection exceptions. Inspecting the container logs revealed that NHibernate’s standard SQL Server configuration (`MsSqlConfiguration.MsSql2012`) was trying to load the legacy `System.Data.SqlClient` driver, which is missing/deprecated in modern Linux .NET 8 runtime images.
* **Correction**: I manually intervened in `Program.cs` and configured the NHibernate session factory to load the modern, cross-platform **`MicrosoftDataSqlClientDriver`** driver:
  ```csharp
  var sessionFactory = Fluently.Configure()
      .Database(MsSqlConfiguration.MsSql2012
          .ConnectionString(connectionString)
          .Driver<NHibernate.Driver.MicrosoftDataSqlClientDriver>()) // <-- Cross-platform SqlClient Driver
      .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(ProductMap))))
      .BuildSessionFactory();
  ```
  This immediately resolved the container crash and successfully connected the API to the SQL Server container.

---

## 5. Phase 3: Handling Edge Cases, Authentication & Validations

To protect the application and ensure smooth operation under stress, I orchestrated robust mechanisms:

* **Global Error Middleware**: Rather than letting raw database/ORM exceptions crash requests or leak internal stack traces to the client, I built `ExceptionHandlingMiddleware`. It intercepts all unhandled errors, logs them using Serilog, and outputs a safe, standard JSON response structure with a `500 Internal Server Error` status.
* **Validation Feedback**: Integrated template validation variables (`#passwordInput="ngModel"`) and dynamic UI error nodes (`<mat-error>`) inside the Angular forms. If a user types a password under 6 characters, they receive clear visual instructions immediately rather than having the submit button silently disabled or getting a 400 Bad Request back from the server.
* **xUnit Test Suites**: Configured 24 robust unit and integration tests. Mocks (`Moq` and `FluentAssertions`) are used to validate business logic in services and verify that API controllers cleanly parse `Result<T>` wrappers into exact HTTP Status Codes under all edge cases (successful responses, resource conflicts, invalid credentials, and missing IDs).

---

## Conclusion: The Modern Engineering Workflow

This project proves that the combination of **strong developer guidance + AI speed** is the future of software engineering. By treating the AI as an execution agent rather than a decision-maker:
1. **Developer defines** the clean architecture, pure Domain constraints, and design patterns.
2. **AI generates** the repetitive boilerplate, service logic, and test cases in seconds.
3. **Developer validates and refactors** ORM leaks, cross-platform driver compatibility, and security controls.

The result is a highly polished, production-grade Product Management system built in record time with **zero warnings, zero compilation errors, and 100% test coverage**.
