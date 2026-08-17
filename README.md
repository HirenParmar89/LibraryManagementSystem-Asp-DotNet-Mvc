Advanced Library Management System
A complete, production-ready enterprise Library Management System built with ASP.NET Core MVC and Clean Architecture. It features role-based authorization, a real-time dashboard, full circulation management (issue, return, renew), fine calculation, reservations, audit logging, and professional reporting.

Table of Contents
Features
Architecture
Technology Stack
Prerequisites
Installation & Configuration
Default Roles & Credentials
Project Structure
License
Features
Core Modules
Dashboard: Real-time statistics, Chart.js visualizations, and recent activities.
Catalog Management: Full CRUD for Books, Book Copies, Authors, Categories, and Publishers.
Circulation: Book Issue, Return, and Renewal workflows with business rule validation (borrowing limits, membership expiry, outstanding fines).
Member Management: Member profiles, membership types, borrowing history, and account activation/deactivation.
Reservation System: Queue management for unavailable books, automatic position calculation.
Fine Management: Automatic fine calculation for overdue books, partial/full payment recording, and fine waiving.
Reporting: Inventory, Circulation, Member, and Fine reports with Excel export and print support.
Notifications: In-app notification system with an unread badge in the navbar.
Audit Logging: Global action filter that automatically logs all write operations (POST/PUT/DELETE) to the database.
Settings: Dynamic system configuration (loan durations, fine amounts, limits) manageable via UI.
Security & Performance
ASP.NET Core Identity: Secure authentication, password hashing, and account lockout.
Role-Based Authorization: Granular permissions for SuperAdmin, Admin, Librarian, Assistant, and Member roles.
Performance Optimization: AsNoTracking() for read-only queries, IMemoryCache for static reference data, and efficient pagination.
Global Exception Handling: Custom middleware and friendly error pages (404, 403, 500).
Serilog: Structured logging to rolling daily files and console.
AJAX: Dynamic barcode lookup during the book issue process.
Architecture
This project follows a strict Clean Architecture (Layered) pattern, ensuring separation of concerns, maintainability, and testability.

LibraryManagementSystem.Domain: Core entities, enums, and constants. No external dependencies.
LibraryManagementSystem.Application: Service interfaces, DTOs, ViewModels, FluentValidation, and business logic implementations. Depends only on Domain.
LibraryManagementSystem.Infrastructure: EF Core DbContext, Repository implementations, Identity stores, Email/SMS implementations. Depends on Application + Domain.
LibraryManagementSystem.Web: MVC Controllers, Razor Views, Filters, Middleware, and wwwroot. Depends on Application + Infrastructure.
Technology Stack
Framework: .NET 10, ASP.NET Core MVC
Database: SQL Server, Entity Framework Core 10
Identity: ASP.NET Core Identity
UI: Bootstrap 5, Font Awesome, Chart.js, DataTables
Logging: Serilog (File & Console Sinks)
Reporting: ClosedXML (Excel Export)
Testing: xUnit, Moq, FluentAssertions
Prerequisites
.NET 10 SDK
SQL Server (or LocalDB)
Visual Studio 2022 / VS Code
Node.js & LibMan (Optional, for client-side library management)
Installation & Configuration
Clone the repository
git clone https://github.com/yourusername/LibraryManagementSystem.gitcd LibraryManagementSystem
Configure Connection String
Open src/LibraryManagementSystem.Web/appsettings.json and update the DefaultConnection to match your SQL Server instance.
Apply Migrations & Update Database
The application automatically applies migrations and seeds the database on startup. To do this manually via CLI:
bash

dotnet ef migrations add InitialCreate --project src/LibraryManagementSystem.Infrastructure --startup-project src/LibraryManagementSystem.Web
dotnet ef database update --project src/LibraryManagementSystem.Infrastructure --startup-project src/LibraryManagementSystem.Web
Run the Application
bash

dotnet run --project src/LibraryManagementSystem.Web
Navigate to http://localhost:5000 (or the URL shown in your console).
Default Roles & Credentials
The database seeder automatically creates the following roles and a default SuperAdmin account:

Roles:

SuperAdmin
Admin
Librarian
Assistant
Member
SuperAdmin Credentials:

Email: superadmin@library.com
Password: SuperAdmin@123
(Note: Please change the SuperAdmin password immediately after your first login in a production environment).

Project Structure
text

LibraryManagementSystem/
│
├── src/
│   ├── LibraryManagementSystem.Domain/       (Entities, Enums, Constants)
│   ├── LibraryManagementSystem.Application/  (Services, DTOs, Interfaces)
│   ├── LibraryManagementSystem.Infrastructure/ (DbContext, Repositories, Identity, Serilog)
│   └── LibraryManagementSystem.Web/          (Controllers, Views, wwwroot)
│
├── tests/
│   └── LibraryManagementSystem.Tests/        (xUnit Tests)
│
├── .gitignore
├── Directory.Build.props
└── LibraryManagementSystem.sln
License
This project is licensed under the MIT License.

text


---

## 2. BUILD VALIDATION

There are no code changes in this phase, but we should verify the solution still builds perfectly.

Run the build command from the root directory:
```bash
dotnet build
Expected Result:
The build should succeed with 0 Error(s) and 0 Warning(s).
