# Database Review

Project: FinanceTracker  
Review Date: 2026-06-04  
Database: PostgreSQL  
ORM: Entity Framework Core

---

# Overview

This document reviews the current database design, relationships, constraints, indexing strategy, and security-related considerations for FinanceTracker.

The objective is to verify data integrity, identify missing protections, and ensure that critical business rules are enforced at the database layer in addition to application-level validation.

---

# Database Schema

## Users

| Column | Type | Constraints |
|----------|----------|----------|
| Id | UUID | Primary Key |
| Email | VARCHAR(72) | Unique, Not Null |
| PasswordHash | TEXT | Not Null |
| CreatedAt | TIMESTAMPTZ | Not Null |

---

## Transactions

| Column | Type | Constraints |
|----------|----------|----------|
| Id | UUID | Primary Key |
| Amount | NUMERIC | Not Null |
| Category | TEXT | Not Null |
| Type | INTEGER | Not Null |
| UserId | UUID | Foreign Key |
| CreatedAt | TIMESTAMPTZ | Not Null |

---

# Entity Relationships

Current relationship model:

Users (1)
│
├───< Transactions (N) (One-to-Many)

Relationship:

Transactions.UserId
→ Users.Id

Delete behavior:

Cascade Delete

---

# Existing Constraints

## Primary Keys

Implemented:

- Users.Id
- Transactions.Id

Purpose:

Provide unique identifiers for all records.

---

## Unique Constraints

Implemented:

- Users.Email

Purpose:

Prevent duplicate user registration.

---

## Foreign Keys

Implemented:

- Transactions.UserId → Users.Id

Purpose:

Guarantee transaction ownership consistency.

---

## Check Constraints

Implemented:

- Amount > 0

Purpose:

Prevent invalid transaction values at database level.

---

# Index Review

## Existing Indexes

### Users

- PK_Users
- IX_Unique_User_Email

### Transactions

- PK_Transactions
- IX_Transactions_UserId

---

## Query Analysis

The majority of application queries filter transactions using:

UserId

Examples:

- GetTransactions
- GetBalance
- DeleteTransaction

The UserId index supports these operations efficiently.

---

# Security Assessment

## Positive Findings

### Database-Level Integrity

Critical ownership relationships are enforced through foreign keys.

### Unique User Registration

Duplicate email addresses are prevented by both application logic and database constraints.

### Defense in Depth

Transaction amount validation exists in both:

- Application layer
- Database layer

### SQL Injection Exposure

Current implementation relies on Entity Framework Core LINQ queries.

No raw SQL execution paths were identified during review.

Risk level:

Low

---

# Open Findings

## DB-001 — Category Length Not Enforced

Severity:

Low

Description:

Category length restrictions currently exist only within application logic.

Risk:

Unexpectedly large values may be inserted directly into the database.

Status:

Open

---

## DB-002 — Transaction Type Constraint Missing

Severity:

Low

Description:

Database does not restrict allowed enum values for transaction type.

Risk:

Unexpected integer values may be stored.

Status:

Open

---

# Recommendations

Short-Term

- Add maximum length constraint for Category.
- Restrict allowed TransactionType values.

Medium-Term

- Review indexing strategy as transaction volume grows.
- Add migration validation checks during CI/CD pipeline.

Long-Term

- Introduce audit logging.
- Add backup and recovery procedures.
- Document database disaster recovery process.

---

# Review Summary

The database provides a solid foundation for application security and data integrity.

Primary keys, unique constraints, foreign key relationships, indexing, and transaction amount validation are implemented.

Remaining findings are low-risk hardening opportunities and do not currently pose a significant threat to application security.
