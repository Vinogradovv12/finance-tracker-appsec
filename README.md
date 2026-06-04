# FinanceTracker

AppSec-oriented finance tracking API built with ASP.NET Core.

## Features

- JWT Authentication
- User Registration/Login
- PostgreSQL Persistence
- Transaction Management
- Rate Limiting
- Global Exception Handling

## Security Features

- Password Hashing
- JWT Authorization
- Ownership Validation
- Rate Limiting
- Input Validation

## Technology Stack

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT Bearer Authentication

## Architecture

Controllers
↓
Services
↓
Data Access
↓
PostgreSQL

## Running Locally

1. Configure appsettings.Development.json
2. Run migrations
3. Start application

```bash
dotnet ef database update
dotnet run