# Security Review – FinanceTracker

## Overview

FinanceTracker is an ASP.NET Web API application for personal finance management. The application uses JWT-based authentication, PostgreSQL for data storage, Entity Framework Core as ORM, and rate limiting mechanisms for authentication endpoints.

Current architecture:

User → Middleware → Controllers → Services → Entity Framework Core → PostgreSQL

---

## Assets

### User Data

- User ID (GUID)
- Email address
- Password hash (BCrypt)
- Account creation date

### Financial Data

- Transaction ID (GUID)
- User ID (GUID)
- Transaction type
- Category
- Amount
- Creation date

### Authentication Assets

- JWT access tokens
- JWT signing secret
- User claims
- Authentication state

### Configuration Assets

- JWT configuration
- Database connection string
- Application settings

### Infrastructure Assets

- PostgreSQL database
- Application logs
- Docker environment (planned)

---

## Threats

### Unauthorized Access

Attackers may attempt to access or modify data belonging to other users.

### Credential Attacks

Attackers may attempt brute-force attacks, password spraying, or user enumeration.

### JWT Abuse

Attackers may attempt to forge, reuse, or manipulate JWT tokens.

### Denial of Service

Attackers may abuse pagination or generate excessive authentication requests.

### Sensitive Data Exposure

Improper logging or error handling may expose sensitive information.

---

## Attack Surface

### Authentication

- POST /api/auth/register
- POST /api/auth/login

### Transactions

- GET /api/transactions
- POST /api/transactions
- DELETE /api/transactions/{id}
- GET /api/transactions/balance

### Infrastructure

- JWT validation pipeline
- Exception middleware
- Rate limiter middleware
- Entity Framework Core database access

---

## Security Controls

### Authentication

- JWT authentication enabled
- Token signature validation enabled
- Token lifetime validation enabled
- BCrypt password hashing

### Authorization

- Protected transaction endpoints
- User identity extracted from JWT claims
- Ownership verification during transaction deletion

### Input Validation

- Email validation
- Password validation
- Transaction amount validation
- Category length validation

### Abuse Protection

- Authentication rate limiting
- Registration rate limiting
- Sliding window rate limiting for transaction endpoints

### Error Handling

- Centralized exception middleware
- Generic authentication error responses
- Generic registration responses

---

## Findings

### F-001: Pagination Abuse

Severity: Medium

Description:

Pagination parameters do not currently enforce upper limits for PageSize values. An attacker may request excessively large result sets and increase resource consumption.

Status:

Verified

Recommendation:

Apply strict validation limits to PageSize and PageNumber.

---

### F-002: Login Timing Differences

Severity: Low

Description:

Authentication flow performs BCrypt verification only when a user account exists. Response timing may differ between valid and invalid email addresses.

Status:

Verified

Recommendation:

Perform password hash verification regardless of account existence.

---

### F-003: Missing Database Constraints

Severity: Low

Description:

Some validation rules exist only within the service layer and are not enforced at database level.

Status:

Verified

Recommendation:

Add database constraints for critical fields.

---

### F-004: Transaction Existence Disclosure

Severity: Low

Description:

Delete operation first loads a transaction and then checks ownership. This may reveal whether a transaction exists before authorization validation.

Status:

Accepted Risk

Recommendation:

Filter by transaction ID and owner ID in a single query.

---

### F-005: Distributed Brute Force

Severity: Low

Description:

Authentication rate limiting is based on client IP address and may be bypassed using multiple IP addresses.

Status:

Accepted Risk

Recommendation:

Consider account-based throttling or additional monitoring controls.

---

## Planned Improvements

### Access Control

- Introduce RBAC model
- Add administrative roles
- Review authorization boundaries

### Security Hardening

- Add pagination limits
- Improve login timing resistance
- Add database-level constraints
- Review JWT issuer and audience validation

### Security Testing

- Perform manual authorization testing
- Execute PortSwigger Academy labs
- Integrate Semgrep
- Integrate Trivy

### Infrastructure

- Dockerize application
- Create docker-compose environment
- Improve deployment reproducibility

---

## Current Security Assessment

The application demonstrates a solid baseline security posture for an educational project.

Implemented protections include:

- JWT authentication
- Password hashing with BCrypt
- Ownership verification
- Rate limiting
- Centralized exception handling
- Input validation

No critical vulnerabilities were identified during the review.

The most significant remaining issue is unrestricted pagination, followed by several low-risk hardening opportunities.

---

## Security Review Update

Date: 2026-06-04

## Resolved Findings

### F-002 Missing Foreign Key

Status: Fixed

### F-003 Missing UserId Index

Status: Fixed

### F-004 Missing Amount Constraint

Status: Fixed