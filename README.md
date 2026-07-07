# PATH

**PATH** is a full-stack team task management application built with ASP.NET Core and Angular. It features organization-scoped workspaces, role-based access control, and a modern reactive frontend.

## Tech Stack

**Backend**
- C#, ASP.NET Core Web API
- Entity Framework Core, SQL Server
- JWT authentication with refresh token rotation and reuse detection

**Frontend**
- Angular 18, TypeScript, SCSS
- Angular Signals for reactive state management
- HTTP Interceptor for silent token refresh

## Features

- **Authentication** — JWT access tokens, HttpOnly cookie refresh tokens, token rotation and reuse detection
- **Organizations** — users are scoped to organizations, admins manage membership
- **Role-Based Access** — organization-level roles (Admin, Manager, Member) enforced backend-wide
- **Projects** — create and manage projects within your organization
- **Tasks** — create, assign, and track tasks with priority and status
- **Guards** — auth guard, guest guard, and role guard protecting all routes

## Project Structure

```
PATH/
├── backend/       # ASP.NET Core Web API
│   ├── PATH.API
│   ├── PATH.Application
│   ├── PATH.Domain
│   └── PATH.Infrastructure
└── frontend/
    └── PATH.Web   # Angular 18
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 20+
- SQL Server

### Backend

```bash
cd backend
dotnet restore
```

Update `appsettings.Development.json` with your connection string and JWT settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server connection string"
  },
  "Jwt": {
    "SecretKey": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  }
}
```

```bash
dotnet ef database update
dotnet run --project PATH.API
```

API available at `https://localhost:7260`

### Frontend

```bash
cd frontend/PATH.Web
npm install
ng serve
```

App available at `http://localhost:4200`



