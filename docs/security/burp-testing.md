# Burp Suite Security Testing

## Overview

This document describes the testing of the Finance Tracker application performed using the DAST tool Burp Suite.

Testing objectives:

Test the application for various vulnerabilities while it is running and examine its behavior in response to different attacks.

Environment:

* Local development
* Docker container
* Buirp Suite Community

---

## T01 - Authentication 

### Objective

Verify endpoint protection and security against unauthorized access

### Procedure

We will test all existing endpoints that require authentication. We will try changing the Authorization header scheme and using invalid JWT tokens.

Existing endpoints requiring authentication:

```Text
GET /api/transactions
GET /api/transactions/balance
POST /api/transactions
DELETE /api/transactions/{id}
```

The specific headers we will try:

```text
1. Empty
2. Authorization: 
3. Authorization: Bearer
4. Authorization: Header
5. Authorization: Bearer hello
6. Authorization: Bearer {expired token}
```

### Result

We obtained the same answer for options 1 through 5 inclusive:

```Text
Response: 401 Unauthorized
In logs: [WARNING] 07/06/2026 10:58:52 : Unauthorized request
```

In 6 option, one can observe the most interesting behavior:

```Text
Response: 
401 Unauthorized
WWW-Authenticate: Bearer error="invalid_token", error_description="The token expired at '05/11/2026 19:45:32'"

In logs: 
[WARNING] 07/06/2026 11:19:01 : JWT failed: IDX10223: Lifetime validation failed. The token is expired. ValidTo (UTC): '05/11/2026 19:45:32', Current time (UTC): '07/06/2026 11:19:01'.
```

### Conclusion

Authentication completed successfully. JWT-based authentication has been adopted as the security measure.


#### Security Controls and Architecture:

* JWT Bearer Authentication
* [Authorize] protection
* Authentication middleware

#### Mapped risk:

OWASP API1:
Broken Object Level Authorization

#### Status:
PASS

---

## TO2 - JWT Validation

Let's verify JWT token validation by modifying various parameters.

### Procedure 

The endpoints from the previous test will be tested.

The mutable JWT parameters will be as follows:

* Payload (exp)
* Signature
* Sub
* Algorithm

We will modify the parameters of the working token on jwt.io and insert the token into the intercepted request in Burp Repeater.

### Result

In all four cases, we will get the same answer:

```Text
Response: WWW-Authenticate: Bearer error="invalid_token", error_description="The signature key was not found"
In logs:
[WARNING] 07/06/2026 12:46:17 : JWT failed: IDX10517: Signature validation failed.
```

However, it is worth noting that if we happen to know the signature key, we will get a different result:


1. Change expire time (1 and 99999999999999)
```Text
1: WWW-Authenticate: Bearer error="invalid_token", error_description="The token expired at '01/01/1970 00:00:01'"

99999999999999: 200 OK
```

2. Change sub (random Guid and real other user id)
   
Random Guid:

```Text
Responses: 200 OK for endpoints: GET. 500 Internal Server Error for POST. 
In logs: 
[ERROR] 07/06/2026 13:04:33 : An error occurred while saving the entity changes. See the inner exception for details.
```

Other user id:

```Text
Response: 200 OK
```

3. Change Alghorithm (from HS256 to HS384)

```Text
Response: WWW-Authenticate: Bearer error="invalid_token", error_description="The signature key was not found"
In logs:
```

### Conclusion

We discovered interesting application behavior when modifying token parameters.
Under normal circumstances—where an attacker does not know the secret key—the application functions correctly. However, the situation becomes more interesting if the attacker discovers the secret key. They can then alter the token's expiration time and perform actions on behalf of other users if their IDs are known. Perhaps the most intriguing scenario arises when a random GUID is injected: the application may return an HTTP 200 response even though the user does not exist (showing a zero balance and no transactions), yet it crashes when a POST request is submitted.

#### Security Controls:

* Storing a secret key in an environment variable
* Short token lifespan

#### Status:

PASS*

---

## T03 Object-Level Authorization (BOLA)

We will check how the application reacts to an attempt to perform an action on other users' transactions.

### Procedure

For this test, we will attempt to delete another user's transaction.

Endpoint: `DELETE /api/transactions/{id}`

Flow:
1. Login as user A
2. Find user B's transaction
3. DELETE /api/transactions/{userBTransactionId}
4. Check result

### Result
 
Response:
```Text
403 Forbiden
{"error":"Forbiden"}
```

In logs:
```Text
[WARNING] 07/06/2026 13:33:14 : User a2ac8d87-f617-4b71-b823-3d43e1805208 attempted to delete someone else's transaction 23e74619-c0dc-4107-85a4-7aad7e0cd5a8
```

### Conclusion

The results show that the application prevented an action involving another user's transaction.
It is also useful that the logs provide information about the attempted operation.

#### Security Controls:

* Use JWT-based authentication
* Event logginп

#### Mapped risk:

OWASP API1:
Broken Object Level Authorization

#### Status:
PASS

---

## T04 Input Validation

In this test, we want to ensure reliable request validation.

### Procedure

We will test the endpoints for transactions: `POST|GET /api/transactions` and the authentication endpoint: `POST /api/auth/login`.

The following tests will be conducted:

```Text
1. Invalid JSON: {email: }
2. Unknown Property: {"email": "b", "isAdmin": true}
3. Invalid Enum: {"type": 999}
4. Invalid Pagination: ?page=-100 and ?pageSize=999999
```

### Result

1. Invalid JSON
```Text
Response: 400 Bad Requestn "error":"Invalid request data"
In logs: [WARNING] 07/06/2026 13:55:38 : Validation failed: $: 'e' is an invalid start of a property name.
```

2. Unknown Property
```Text
Response: 400 Bad Request "error":"Invalid request data"
In logs: [WARNING] 07/06/2026 13:58:20 : Validation failed: request: The request field is required.
```

3. Invalid Enum
```Text
Response: 400 Bad Request "error":"Invalid request data"
In logs: [WARNING] 07/06/2026 14:01:25 : Validation failed: request: The request field is required.
```

4. Invalid Pagination

pageSize=999999:
```Text
Response: 200 OK
In logs: 
```

?page=-100:
```Text
Response: 500 Internal Server Error
In logs: [ERROR] 07/06/2026 14:08:23 : 2201X: OFFSET must not be negative
```

### Remediation

The pagination vulnerability was fixed and re-verified. Validation using Data Annotations was employed to resolve the issue.

#### Implementation

Use Data Annotations in PaginationRequests:
```cs
public record PaginationRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; init; } = 20;
}
```

### Conclusion

The results were mixed. On the one hand, most invalid requests resulted in a 400 error.
On the other hand, a pagination issue arose: an attacker could specify an excessively large value, placing a heavy load on the server. The pagination issue has since been resolved and verified.

#### Security Controls

* Use of special contracts for requests and responses
* Use mapping

#### Status

Object validation: PASS
Pagination validation: PASS

---

## T05 Business Logic Testing

In this test, we will examine the operation of the business logic itself in a bit more depth.

### Procedure

We will work with the transaction creation endpoint `POST /api/transactions`.

We will verify not just the validity of the request, but specifically the response to different values ​​of `Amount`.

Let us consider the following values:

```Text
0
-100
9999999999
0.00000001
```

### Result

We obtained a consistent result across all tests.

```Text
Response: 400 Bad Request "error":"Invalid request data"
In logs: [WARNING] 07/06/2026 14:26:03 : Validation failed: Amount: The field Amount must be between 0.01 and 1000000.
```

### Conclusion

The validation worked perfectly and demonstrated its reliability.

#### Security Controls

* Use of multi-level validation (request, service, domain)

#### Status

PASS

---

## T06 Information Disclosure

The test must verify that the application does not expose user information, even in logs.

### Procedure

The auth endpoints `POST /api/auth/login|registration` will be tested.

We will compare response length and time for existing and non-existing users.

### Result

Login endpoint:

User not exist:
```Text
Response: 401 Unauthorized "message":"Invalid email or password."
Time: 11 ms
Content-Length: 40
In logs: [WARNING] 07/06/2026 15:00:30 : Failed login attempt for user4@gmail.com with provided password.
```

User exist:
```Text
Response: 401 Unauthorized "message":"Invalid email or password."
Time: 228 ms
Content-Length: 40
In logs: [WARNING] 07/06/2026 14:57:37 : Failed login attempt for user3@gmail.com with provided password.
```

Register endpoint:

User not exist:
```Text
Response: 200 OK "message": "User registered successfully"
Time: 351 ms 
Content-Length: 41
In logs: [INFO] 07/06/2026 14:48:50 : New user registered: user3@gmail.com
```

User exist:
```Text
Response: 200 OK "message":"Please check your email and confirm your registration"
Time: 153 ms
Content-Length: 67
In logs: [WARNING] 07/06/2026 14:58:58 : Attempt to register with user3@gmail.com which already exists.
```

### Conclusion

Based on the results, we can see that an attacker can rely on request length and timing to determine the existence of a user. Excessive logging is also noticeable, where every failed attempt is recorded along with the email address.


#### Necessary requirements for improvement:

* Fixed response time
* Fixed response length
* Log balancing

#### Status:

Not PASS

---

## T07 Rate Limiting

### Procedure

We will test the rate limiter by sending a large number of requests and then compare the results for different endpoints. The test will cover all endpoints.

The attack will be carried out using Burp Intruder.

### Result

**Login endpoint:**

429 Too Many Requests
Limit: 5 request

**Register endpoint:**

429 Too Many Requests
Limit: 3 request

**Transactions endpoints:**

429 Too Many Requests
limit: 10 request

### Conclusion

The result turned out excellent, but there are limitations and ideas for improvement.

#### Security Controls

* Using different policies for different endpoints
* Sliding window policy 

#### Limits 

There is currently a vulnerability to mass attacks involving IP rotation. Additionally, the rate limiter's timing data persists for the lifetime of the application itself.

#### Future Improvments

* Storing time in Redis

#### Status

PASS

---

## T08 CORS Review

### Procedure

To verify, we will intercept the request and inject the header `Origin: https://evil.com`.

### Result

Requests with and without the Origin header yielded identical responses. There were no extra headers, such as Access-Control-Allow-Origin.

#### Security Controls

* Use of a configured CORS policy

#### Status

PASS

---

## T09 HTTP Methods

### Procedure

We will test the application's behavior when using specific HTTP methods.

Methods under test:
* PUT
* PATCH
* HEADER
* OPTIONS

### Result

An identical response was obtained in all results.

405 Method Not Allowed
Allow: GET, POST

#### Status

PASS

---

## Findings

During testing, some interesting cases were identified.
Some involve minor vulnerabilities, others were left unresolved intentionally, while some represent serious vulnerabilities that need to be fixed.

| ID | Severity | Finding | Recommendation | Status | 
| --- | --- | --- | --- | --- |
| F01 | High | Pagination validation | Validate Page/PageSize | Fixed |
| F02 | Medium | Timing differences | Constant response timing | Accepted |
| F03 | Low | Rate limiting bypass with IP rotation | Distributed limiter | Accepted |
| F04 | Low | Changing the JWT `sub` | Validate `sub` | Accepted |