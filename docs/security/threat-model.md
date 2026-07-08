# Threat Model

## Purpose

This document identifies
potential threats against Finance Tracker
and describes implemented mitigations.

Methodology:

**STRIDE**

---

## System Overview

Actors:

* Anonymous User
* Authenticated User

Assets:

* User accounts
* Password hashes
* JWT tokens
* Financial transactions
* Authentication secrets
* Database integrity

Trust Boundaries:

```Text
Internet
↓
REST API
↓
Application Services
↓
Database
```

---

## Threat Analisys

### S - Spoofing Identity

Examples:

* Credential stuffing
* JWT theft
* Token replay

Mitigations:

* Argon2id
* JWT validation
* Token expiration

---

### T - Tampering

Examples:

* Transaction modification
* JSON parameter manipulation
* Enum tampering

Mitigations:

* Ownership validation
* Strict JSON deserialization
* Data validation

---

### R - Repudiation

Examples:

* Denial of an attempt to access others transactions
* Denial to delete transaction

Mitigations:

* Validation logging
* Authentication events

---

### I — Information Disclosure

Examples:

* BOLA
* Detailed error messages
* Enumeration

Mitigations:

* Generic authentication responses
* Centralized exception handling
* Ownership validation

---

### Denial of Service

Examples:

* Login brute force
* API abuse

Mitigations:

* Rate limiting


---

### Elevation of Privilege

Examples:

* Access others transactions
* JWT manipulation

Mitigations:

* JWT validation
* Ownership verification

---

## OWASP Mapping

| Threat | OWASP |
|--------|-------|
| BOLA | API1 |
| Broken Authentication | API2 |
| Excessive Data Exposure | API3 |
| Unrestricted Resource Consumption | API4|
| Security Misconfiguration | API8 |

---

## Security Review Summary

Current posture:

Authentication:
STRONG

Authorization:
GOOD

API Security:
GOOD

Input Validation:
GOOD

Operational Security:
BASIC

Production Readiness:
MEDIUM