# TaskFlow — .NET 8 MVC Project Management App

A full-featured **Task & Project Management** web application built with ASP.NET Core 8 MVC, Entity Framework Core, and SQL Server. Designed to showcase professional-grade .NET development skills for your resume.

---

## ✨ Features

| Feature | Details |
|---|---|
| **Authentication** | ASP.NET Core Identity — Register, Login, Logout, Account Lockout |
| **Project Management** | Create, edit, delete projects with color-coding and progress tracking |
| **Kanban Board** | Drag-and-drop task board with Todo / In Progress / Review / Done columns |
| **Task Management** | Full CRUD — assign tasks, set priorities, due dates, track status |
| **Comments** | Leave comments on tasks for team collaboration |
| **Dashboard** | Stats overview with Chart.js doughnut and bar charts |
| **REST API** | AJAX-powered status updates without page reloads |
| **Responsive UI** | Mobile-friendly layout with sidebar navigation |

---

## 🏗️ Architecture

```
TaskFlow/
├── Controllers/           # MVC + REST API endpoints
│   ├── AccountController  # Auth: Register, Login, Logout
│   ├── DashboardController
│   ├── ProjectsController # CRUD + JSON API
│   └── TasksController    # CRUD + AJAX UpdateStatus endpoint
│
├── Models/                # Domain models (EF Core entities)
│   └── Models.cs          # ApplicationUser, Project, ProjectTask, Comment, etc.
│
├── ViewModels/            # DTOs for Views (separation of concerns)
│   └── ViewModels.cs
│
├── Data/
│   └── ApplicationDbContext.cs  # EF Core Identity + custom entities
│
├── Interfaces/            # Service abstractions (Dependency Inversion)
│   └── IServices.cs
│
├── Services/              # Business logic layer
│   ├── ProjectService.cs
│   ├── TaskService.cs
│   └── DashboardService.cs
│
├── Views/                 # Razor views
│   ├── Shared/_Layout.cshtml
│   ├── Account/           # Login, Register
│   ├── Dashboard/         # Dashboard with Charts
│   ├── Projects/          # Index, Details (Kanban), Create, Edit
│   └── Tasks/             # Create, Details, MyTasks
│
└── wwwroot/
    ├── css/site.css        # Custom CSS design system
    └── js/site.js          # Chart.js charts + AJAX calls + Kanban drag & drop
```

---

## 🛠️ Technology Stack

- **Framework:** ASP.NET Core 8 MVC
- **Database:** SQL Server + Entity Framework Core 8
- **Auth:** ASP.NET Core Identity (cookie-based)
- **ORM:** EF Core with Code-First migrations
- **Frontend:** Bootstrap 5.3, Bootstrap Icons, Chart.js 4
- **Fonts:** Plus Jakarta Sans (Google Fonts)
- **Design Pattern:** Repository/Service pattern with DI

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB is fine for development)
- Visual Studio 2022 or VS Code

### Setup

```bash
# 1. Clone / open the project
cd TaskFlow

# 2. Restore packages
dotnet restore

# 3. Update the connection string in appsettings.json
#    Default uses SQL Server LocalDB — no changes needed for VS development

# 4. Apply migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# 5. Run the application
dotnet run
```

The app will open at `https://localhost:5001` — register a new account to get started.

---

## 🔑 Key Technical Concepts Demonstrated

### 1. Clean Architecture
- Controllers are thin — all business logic lives in Services
- Interfaces allow easy unit testing and dependency injection
- ViewModels separate domain models from UI concerns

### 2. Entity Framework Core
- Code-First database with relationships (one-to-many, many-to-many)
- Navigation properties, eager loading with `.Include()`
- Configures cascade delete and restrict behaviors in `OnModelCreating`

### 3. ASP.NET Core Identity
- Custom `ApplicationUser` extending `IdentityUser`
- Cookie authentication with sliding expiration
- Account lockout after failed attempts

### 4. REST API + AJAX
- `POST /Tasks/UpdateStatus` — JSON endpoint consumed via JavaScript `fetch()`
- `GET /Projects/GetProjectsJson` — Returns project list as JSON
- Anti-Forgery token sent in AJAX headers for security

### 5. Authorization
- `[Authorize]` attribute on all authenticated controllers
- Service-layer checks ensure users can only access their own data

---

## 📊 Database Schema

```
ApplicationUsers  ──< ProjectMembers >── Projects
                                              │
                                          ProjectTasks
                                              │
                                          Comments
```

---

## 📸 Pages Overview

- `/Account/Login` — Login page
- `/Account/Register` — Registration page  
- `/Dashboard` — Stats, Charts, Recent Projects, My Tasks
- `/Projects` — Project grid with progress bars
- `/Projects/Details/{id}` — Kanban board with drag & drop
- `/Projects/Create` — Create new project with color picker
- `/Tasks/Details/{id}` — Task detail with comments
- `/Tasks/MyTasks` — All tasks assigned to current user

---

## 💡 Resume Talking Points

- "Built a full-stack web application using ASP.NET Core 8 MVC with Entity Framework Core and SQL Server"
- "Implemented service-layer architecture with dependency injection following SOLID principles"
- "Integrated ASP.NET Core Identity for secure cookie-based authentication with account lockout"
- "Created REST API endpoints consumed via AJAX for real-time task status updates"
- "Developed interactive Kanban board with drag-and-drop using vanilla JavaScript"
- "Designed responsive dashboard with Chart.js visualizations for project analytics"
