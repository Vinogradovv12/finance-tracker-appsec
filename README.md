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

## Deployment & Running Locally

### Prerequisites

- [Docker Desktop](https://docker.com)
- [Docker Compose](https://docker.com)

### Quick Start

1. **Clone the repository**
2. **Setup Environment Variables:**
   Create a `.env` file in the root directory based on the `.env.example` file:
   ```bash
   cp .env.example .env
   ```
   *Note: Open `.env` and change the default passwords to secure ones.*

3. **Run the containers:**
   ```bash
   docker-compose up --build -d
   ```

The API will be available at `http://localhost:9090`.

### Environment Variables Reference

| Variable | Description |
| :--- | :--- |
| `POSTGRES_USER` | Admin username for PostgreSQL |
| `POSTGRES_PASSWORD` | Admin password for PostgreSQL |
| `POSTGRES_DB` | Database name |
| `ConnectionStrings__Default` | Connection string for the API to connect to `postgres` service |
| `JwtOptions__SecretKey` | Secret key used for signing JWT tokens |
| `JwtOptions__ExpiresHours`| JWT token lifetime in hours |
| `JwtOptions__Issuer`| Who created the token |
| `JwtOptions__Audience`| Who the token is intended for |
