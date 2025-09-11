### MinaretOps Server (Backend API)

A modern, modular backend built on ASP.NET Core that streamlines client delivery, internal operations, communications, and insights—engineered with OOP best practices and clean layering to be reliable today and flexible to scale at any size.

- **Tech stack**: ASP.NET Core Web API, EF Core (SQL Server), ASP.NET Core Identity, JWT Auth, AutoMapper, MailKit/MimeKit, Web Push (VAPID), ImageSharp, Discord webhooks
- **Architecture**: Clean, layered solution (`Core` → `Infrastructure` → `Client API`)
- **Domains**: Auth, Clients & Services, Tasks, Internal Tasks, Attendance & Leave, Announcements, Notifications, Complaints, KPI, Contact Forms, Blog & Portfolio, Reporting
- **Engineering**: Implements OOP concepts, SOLID-inspired abstraction via `Core.Interfaces`, DTO-first design, dependency injection for testability and scale

---

## ✨ Features

- **Authentication & Authorization**
  - JWT-based login and refresh; ASP.NET Core Identity with roles and token providers
  - Secure password flows and user lifecycle management

- **Client & Service Management**
  - Full CRUD for clients and services; link services to clients (`ClientService`)
  - Lightweight lists and full details via dedicated DTOs

- **Tasks & Workflows**
  - Task items with grouping (`TaskGroup`) for projects/sprints
  - Client task reporting and monthly summaries

- **Internal Operations**
  - Internal tasks with employee assignments and progress tracking

- **Attendance & Leave**
  - Daily attendance records and leave request lifecycle

- **Announcements**
  - Organization-wide announcements with employee targeting (`EmployeeAnnouncement`)

- **Notifications**
  - Web Push notifications (VAPID), subscription handling (`CustomPushSubscription`)
  - Discord integration for ops alerts and events

- **Complaints**
  - Intake and tracking of complaints linked to employees and incidents

- **KPI & Incidents**
  - KPI incident reporting (`KPIIncedint`), employee monthly KPI summaries

- **Contact Forms**
  - Secure intake pipeline from website forms to backend for follow-up

- **Content (Blog & Portfolio)**
  - Blog categories and posts; portfolio categories and projects

- **Media**
  - Accepts any image, transforms to `.webp`, enhances quality, and renames to a friendly SEO-safe name

- **Reporting**
  - Task and client productivity reports with purpose-built DTOs

---

## 🧱 Architecture

A clean, layered structure that isolates domain models from infrastructure details and exposes a lean API surface.

```
Core/
  - Models (Entities: ApplicationUser, Client, TaskItem, KPIIncedint, ...)
  - DTOs (Contracts per feature: Auth, Clients, Tasks, Reports, ...)
  - Interfaces (Service contracts)
  - Settings (Typed options: JWT, Email)

Infrastructure/
  - Data (DbContext, EF configurations, migrations)
  - Services (Implementations: Auth, Clients, Tasks, Attendance, KPIs, ...)
  - MappingProfiles (AutoMapper profiles)
  - EmailTemplates (HTML templates)
  - Integrations (Email, Discord, Web Push, Media)

Client API/
  - Controllers (Auth, Clients, Tasks, KPI, Notifications, ...)
  - Program (DI wiring, middleware, Identity, JWT, Swagger)
```

- **Flow**: Controllers → `Core.Interfaces` → `Infrastructure.Services` → `MinaretOpsDbContext`
- **Mapping**: AutoMapper between `Core.Models` and `Core.DTOs`
- **Security**: ASP.NET Core Identity + JWT
- **Integrations**: MailKit/MimeKit (email), Web Push (VAPID), Discord webhooks, ImageSharp (media)

---

## 🧩 Packages & Integrations

- **ASP.NET Core**
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
  - `Microsoft.AspNetCore.OpenApi` (Swagger)
- **Data**
  - `Microsoft.EntityFrameworkCore`, `SqlServer`, `Relational`
- **Security**
  - `Microsoft.IdentityModel.*`, `System.IdentityModel.Tokens.Jwt`
- **Mapping**
  - `AutoMapper`
- **Email**
  - `MailKit`, `MimeKit`
- **Push**
  - `Lib.Net.Http.WebPush`, `Lib.Net.Http.EncryptedContentEncoding`, `BouncyCastle.Cryptography`
- **Media**
  - `SixLabors.ImageSharp`
- **Utilities**
  - `Microsoft.Extensions.*`

---

## 🔐 Security

- JWT bearer tokens with refresh token support (`RefreshToken`)
- ASP.NET Core Identity for user/role management
- Strict DTO boundaries and DI for secure composition
- CORS-configurable for frontend integrations

---

## 🚀 API Modules (Controllers)

- `AuthController`
- `ClientController`, `ServiceController`
- `TasksController`, `InternalTaskController`
- `AttendanceController`
- `AnnouncementController`
- `NotificationController`
- `ComplaintController`
- `KPIController`
- `ContactFormController`

Each controller uses DTOs for strongly typed request/response contracts, backed by services registered in DI in `Program.cs`.

---

## ⚙️ Configuration

Provide the following in `Client API/appsettings.json` (or environment variables):

- `ConnectionStrings:DefaultConnection` (SQL Server)
- `JWT:...` (issuer, audience, key, durations)
- `EmailSetting:...` (SMTP host, port, credentials, sender)
- `Push:...` (VAPID public/private keys)
- Optional: `Discord:WebhookUrl`

---

## 🛠️ CI/CD Workflow (Automatic Deployment)

This project is ready for automated delivery using a standard CI pipeline (e.g., GitHub Actions or Azure DevOps):

- **Triggers**
  - On `push` to `main` or `release/*`
  - On pull requests for validation

- **Jobs**
  - Restore dependencies and cache
  - Build and run unit/integration tests
  - Build Docker image and tag with commit SHA and semver
  - Push image to container registry (e.g., GHCR/ACR)
  - Run EF Core migrations (if enabled)
  - Deploy to target environment (e.g., Azure Web App for Containers, AKS, or on-prem Docker host)

- **Environment gates**
  - Manual or approval-based promotion from `staging` to `production`

---
<!--
## 📹 Live Demo (Video)

- [Add your demo video link here]

---
-->
## 🖥️ Frontend

- Source repo: [MinaretOps-Client -- Frontend Repo](https://github.com/abdofathy883/MinaretOps-Client)
- The frontend integrates with this backend via REST endpoints and uses JWT for secure sessions.

---

## 🧑‍💻 Developer

- **Name**: Abdelrahman Fathy
- **Role**: Full-Stack/.NET Engineer
- **About**: I build scalable, maintainable systems using clean architecture, OOP best practices, and reliable integrations that accelerate business outcomes.
- **Contact**: [LinkedIn Account](https://www.linkedin.com/in/abdelrahman-fathy-dev/)

---

## 📁 Repository Structure

```
MinaretOps-Server/
  Client API/
    Controllers/
    Program.cs
    appsettings*.json
    Dockerfile
  Core/
    DTOs/
    Interfaces/
    Models/
    Settings/
  Infrastructure/
    Data/
    Migrations/
    MappingProfiles/
    Services/
    EmailTemplates/
  docker-compose.yml
  MinaretOps.sln
```
