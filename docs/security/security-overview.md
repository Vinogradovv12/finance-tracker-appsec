# Security Overview

## Purpose

FinanceTracker is a security-focused REST API for personal finance management developed with ASP.NET Core.

The project demonstrates practical implementation of Secure SDLC principles through secure authentication, authorization, input validation, defensive API design and layered application architecture.

Security controls are integrated into the application architecture rather than added as isolated features.

---

## Security Objectives

The application was developed to achieve the following goals:

* Protect API endpoints from unauthorized access
* Prevent Broken Access Control
* Ensure secure password storage
* Validate all client-supplied data
* Reduce attack surface through layered architecture
* Provide consistent and safe API responses
* Support secure application maintenance

---

## Security Architecture

Application security is implemented in several layers.

```Text
Client
↓
Controllers
↓
Authentication layer
↓
Authorization layer
↓
Service Layer
↓
Infrastructure
↓
Entity Framework Core
↓
PostgreSQL
```

A specific security measures applied to the application layer is presented in the following table.

| Layer | Security Responsibility |
| :---: | :---: |
| Controllers | Request processing and model binding |
| Authentication | JWT validation |
| Authorization | Ownership verification |
| Services | Business logic enforcement |
| Validation | Request validation |
| Middleware | Centralized exception handling |
| Infrastructure | Password hashing and token generation |
| Database | Persistent storage and data integrity |

---

## Implemented Security Controls

### API Security

Controls:

* JWT Bearer Authentication
* Protected endpoints
* Ownership validation
* Strict JSON deserialization
* Pagination support
* Consistent HTTP status codes

Threats reduced:

* Broken Object Level Authorization (BOLA)
* Broken Authentication
* Excessive Data Exposure
* Mass Assignment
* Injection risks

---

### Authentication

Controls:

* JWT Bearer Authentication
* Argon2id password hashing
* Random cryptographic salt
* Configurable hashing parameters
* Constant-time password comparison
* Email normalization

Threats reduced:

* Credential disclosure
* Password cracking
* Timing attacks
* Session impersonation

---

### Authorization

Authorization decisions are performed before business logic execution.

Controls:

* JWT claims validation
* Ownership-based authorization
* Protected transaction endpoints

Threats reduced:

* IDOR
* Broken Object Level Authorization

---

### Input Validation

Controls:

* Data Annotations
* **JSON strict deserialization** – Uses `JsonUnmappedMemberHandling.Disallow` to block any payloads with unexpected fields, preventing Mass Assignment.
* **Enum validation** – Employs `JsonStringEnumConverter` without integer support to force strict string matching and prevent out-of-range numeric injections.
* Pagination constraints
* Automatic model validation

Threats reduced:

* Invalid requests
* **Parameter tampering** – Manipulation of internal states, type confusion, or unexpected enum values.
* **Unexpected JSON properties** – Over-posting and mass assignment attempts targeting internal database fields.

---

### Error Handling

Application uses centralized exception handling.

Controls:

* Global exception middleware
* Domain exceptions
* Generic client responses
* Validation logging

Custom Domain Exceptions:

* Invalid Credentials
* User Already Exists

Threats reduced:

* Information disclosure
* Inconsistent error behavior

---

### Rate Limiting

Authentication endpoints use a dedicated rate-limiting policy based on a combination of client IP address and request path.

Purpose:

* Reduce brute-force attempts
* Reduce abuse
* Limit repeated requests

Threats reduced:

* Brute force
* Automated abuse

---

### Logging

Application events are logged.

Examples:

* Authentication events
* Validation failures
* Domain exceptions
* Access violations

Principlies:

* Sensitive information such as passwords and JWT secrets is never logged.

---

## Security Design Decisions

Decision:

Use Argon2id instead of BCrypt.

Reason:

Follow modern password hashing recommendations and allow configurable hashing parameters.

–

Decision:

Reject unknown JSON properties.

Reason:

Reduce attack surface and prevent unexpected input.

–

Decision:

Normalize email addresses before persistence.

Reason:

Prevent duplicate identities caused by case differences.

–

Decision:

Use ownership validation instead of client-supplied identifiers.

Reason:

Reduce the risk of Broken Object Level Authorization (BOLA).

## Known Limitations

The current implementation does not yet include:

* Refresh Tokens
* Multi-factor authentication
* Token revocation
* Security headers
* Automated SAST/DAST
* Security-focused CI pipeline

These improvements are planned for future iterations.

---

## Future Improvements

Plans include adding the following improvements:

* Gitleaks scanning
* OWASP ZAP assessment
* GitHub Actions security pipeline
* Threat modeling expansion
* Security headers
* Refresh token support

---

## References

* OWASP Top 10 (2021)
* OWASP API Security Top 10 (2023)
* RFC 7519 — JSON Web Token
* RFC 9106 — Argon2 Password Hashing
* ASP.NET Core Security Documentation
* Entity Framework Core Documentation