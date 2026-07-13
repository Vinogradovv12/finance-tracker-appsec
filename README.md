# FinanceTracker

### Secure Personal Finance REST API built with ASP.NET Core

FinanceTracker is a secure REST API for personal finance management built with ASP.NET Core and PostgreSQL.

The project was created as a practical implementation of secure backend development concepts including authentication, authorization, secure password storage, ownership-based access control, input validation, exception handling and secure API architecture.

It serves as an Application Security portfolio project demonstrating Secure SDLC principles and modern backend engineering practices.

---

## Security Overview

Finance Tracker was developed with security-focused design principles.

Implemented controls:

### Authentication

* JWT Bearer Authentication
* Argon2id password hashing
* Token validation and expiration
* Email normalization

### Authorization

* Ownership-based authorization
* Protected endpoints
* JWT claims validation

### Aplication Security

* Data Annotations validation
* Strict JSON deserialization
* Centralized exception handling
* Constant-time password verification
* Rate limiting
* Structured logging
* Global validation handling
* Environment-based configuration

### Secure Development Practices

* Layered architecture
* Dependency Injection
* Service abstraction
* Environment-based secrets
* Explicit DTOs
* Entity separation
  
---

## Security Testing

The application security was validated using both manual and automated testing techniques.

Implemented testing includes:

- Manual API security testing (Burp Suite)
- Static Application Security Testing (Semgrep)
- Dynamic Application Security Testing (OWASP ZAP)
- Secret Detection (Gitleaks)

All findings, remediation steps, and verification results are documented under `docs/security/`.

---

## Project Overview

### Authentication

* User registration
* User login
* JWT token generation

### Transactions

* Create transaction
* Delete transaction
* Paginated transaction list
* Personal balance

### API

* RESTful endpoints
* OpenAPI documentation
* Scalar UI
* Docker deployment


---

## Architecture

Application follows layered architecture.

```Text
Client
   ↓
Controller Layer
   ↓
Service Layer
   ↓
Infrastructure
   ↓
Entity Framework Core
   ↓
PostgreSQL
```

Project Structure:

```Text
FinanceTracker.api/
├── Contracts/
├── Controllers/
├── Data/
├── Domain/
├── Exceptions/
├── Extensions/
├── Infrastructure/
├── Middleware/
├── Migrations/
├── Services/
├── docker/
├── docs/
└── Program.cs
```

---

## Technology Stack

### Backend 

* ASP.NET Core 9
* Entity Framework Core (Parameterized Queries)
* PostgreSQL

### Security

* JWT Bearer Authentication
* Argon2id
* ASP.NET Rate Limiting
* Data Annotations
* Structured Logging
* Security Headers

### Infrastructure

* Docker
* Scalar OpenAPI

---

## Getting Started

### Prerequisites

- [Docker Desktop](https://docker.com)
- [Docker Compose](https://docker.com)

### Quick Start

1. **Clone the repository**
   ```Bash
   git clone https://github.com/ivanov-appsec/finance-tracker-appsec
   cd finance-tracker-appsec
   ```

2. **Setup Environment Variables:**
   Create a `.env` file in the root directory based on the `.env.example` file:
   ```bash
   cp .env.example .env
   ```
   *Note: Open `.env` and change the default passwords to secure ones.*


   **Environment Variables Reference**

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
   | `Argon2Options__SaltSize` | Size of the salt in bytes (Default: 16) |
   | `Argon2Options__DegreeOfParallelism` | Number of CPU threads Argon2 will use (Default: 4) |
   | `Argon2Options__Iterations` | Number of memory passes Argon2 will perform (Default: 3) |
   | `Argon2Options__MemorySize` | Memory pool size for Argon2 in KB, e.g., 65536 for 64MB (Default: 65536) |
   | `Argon2Options__HashSize` | Size of the generated password hash in bytes (Default: 32) |

3. **Run the containers:**
   ```bash
   docker compose up --build -d
   ```

The API will be available at `http://localhost:9090`.

---

## Security Documentation

Security documentation is located in `docs/security/`

Documents:

* [security-overview.md](docs/security/security-overview.md)
* [auth-model.md](docs/security/auth-model.md)
* [threat-model.md](docs/security/threat-model.md)
* [burp-testing.md](docs/security/burp-testing.md)
* [sast-report.md](docs/security/sast-report.md)
* [dast-report.md](docs/security/dast-report.md)

---

## Future Improvements

### Authentication

* Refresh Tokens
* Refresh Token Rotation

### DevSecOps

* GitHub Actions CI Pipeline
* Automated SAST (Semgrep)
* Automated Secret Detection (Gitleaks)
* Automated DAST (OWASP ZAP Baseline)

### Security

* Content Security Policy
* Security Headers Expansion
* Audit Logging Improvements

### Infrastructure

* Redis-backed Rate Limiting