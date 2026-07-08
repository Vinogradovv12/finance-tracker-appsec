# Authentication & Authorization Model

## Purpose

This document describes authentication, authorization,
identity flow, and access control mechanisms implemented
inside Finance Tracker.

Goal:
prevent unauthorized access, privilege escalation,
and object-level authorization issues.

---

## Authentication Flow

Finance Tracker uses stateless JWT 

```Text
Client
↓
POST /login
↓
Credential Validation
↓
Argon2 Password Verification
↓
JWT Generation
↓
Authorization Header
↓
Protected Endpoint
↓
JWT Validation
↓
Current User Context
```

---

## Authentication Components

### Password Storage

Passwords are stored as hashes.

Implementation:

* Argon2id
* Configurable parameters
* Random cryptographic salt
* Constant-time verification

Hash and verify flow:

```Text
Register: Client → AuthController.Register → AuthService (Validate & Unique Check) →
PasswordHasher (Argon2id + Salt) → EF Core (Users DB) 

Login: Client → AuthController.Login → AuthService (Fetch User) →
PasswordHasher (FixedTimeEquals Check) → IJwtProvider → Token 
```

Security benefits:

- [x] Brute-force resistance  
- [x] Password leakage mitigation
- [x] Constant-time hash comparison
- [x] Rainbow table resistance  

---

### Token-Based Authentication

Authentication uses JWT access tokens.

JWT claims:

```JSON
{
    "sub": UserId (GUID)
}
```

Validation:

```Text
Request
↓
Authorization Header
↓
JwtBearerMiddleware
↓
Secret key verification
↓
Claims validation
↓               ↓
200 (OK)        401 (Unauthorized)
```

Arhitetecture:

Authentication and aurhorizarion service configuration has been moved to a separate extension (ApiExtensions):

```Text
services.AddAuthentication(options => ...
.AddJwtBearer
    options.TokenValidationParameters
    ...
    options.Events
    ...
```

Register the middleware in Program.cs:

```Text
app.UseAuthentication();
app.UseAuthorization();
```

Add the attribute to the controllers:
```Text
[Authorize]
public class TransactionController : ControllerBase
```

Security controls:

- [x] Signature verification 
- [x] Invalid token rejection
- [x] Expiration validation
- [x] Centralized Auth Dependency

---

## Authorization Model (Ownership-Based Access Control)

Authorization Strategy

FinanceTracker does not implement role-based authorization. Instead, authorization follows an ownership-based access control model.

Protected resources are always queried using both:

* Resource identifier
* Current authenticated user

Implementation:

```Text
Client
↓
JWT Validation
↓
Extract UserId
↓
Transaction Lookup
↓
Transaction.Owner == UserId
↓
ALLOW / DENY
```

---

## Object-Level Authorization

Finance Tracker validates access to specific objects. Every operation involving a transaction validates ownership before executing business logic.

Example:

```text
User A
↓
Delete User B transaction
↓
Transaction.Owner == UserId
↓
403 (Forbidden)
```

Mitigates:

OWASP API1:
Broken Object Level Authorization (BOLA).

---

## Session Model

Authentication state:

```Text
Client
↓
JWT Header
↓
Stateless Validation
↓
Database Lookup
```

The server does not store session state.

---

## Security Decisions

Decision:
Ownership validation instead of role checks.

Reason:
The application manages user-specific resources rather than shared resources.

---

Decision:
Argon2id for password hashing.

Reason:
Memory-hard algorithm recommended for password storage.

---

Decision:
Stateless JWT authentication.

Reason:
Simplifies API scalability.

Tradeoff:
Token revocation requires additional mechanisms.

---

## Future Improvements

Planned:

* Refresh Tokens
* Secure Cookies
* Token Rotation
* Multi-Factor Authentication
* Session Revocation