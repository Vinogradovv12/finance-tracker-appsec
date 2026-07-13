# DAST Security Report

## Purpose

This document presents the results of an analysis conducted using DAST scanners. The objective was to identify potential security issues in the running application, such as Injection, XSS, missing security headers, and information disclosure.

---

## Methodology

The analysis was performed using DAST scanners, and all findings were manually verified to distinguish them from false positives. 

Risk mitigation measures were also implemented for the identified vulnerabilities, followed by a re-verification.

### Lifecycle

```Text
Discovery
↓
Authenticated Scan
↓
Finding Validation
↓
Remediation
↓
Verification
↓
Documentation
```

---

## Environment

* Local development
* Docker container

---

## Tool Configuration

**OWASP ZAP**

Mode:
Authenticated Scan

Spider:
AJAX Spider

API Definition:
Scalar OpenAPI

Authentication:
Bearer Token

Target:
http://localhost:9090

---

## Summary

**Overall scanner result**

| Tool | Findings |
| --- | :---: |
| OWASP ZAP | 2 |

**Findings**

| ID | Rule | Severity | Status |
| :---: | --- | --- | --- |
| DAST-01 | Missing X-Content-Type-Options | Low | Fixed |
| DAST-02 | Authentication Request Identified | Informational | Verified |

---

## Findings

### DAST-01 – Missing X-Content-Type-Options

#### Tool
`OWASP ZAP`

#### Severity
`Low`

#### CWE
```Text
CWE-693: Protection Mechanism Failure
```

#### OWASP
```Text
OWASP Top 10 2021: A05 – 
OWASP Top 10 2025: A02 – Security Misconfiguration
```

#### Vulnerability Details

URL: `http://localhost:9090/api/transactions`

Method: `GET`

Request:
```HTTP
GET http://localhost:9090/api/transactions HTTP/1.1
Host: localhost:9090
Authorization: Bearer <token>
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64)
```

Response:
```HTTP
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Content-Length: 73
Server: Kestrel

{"items":[],"pageNumber":1,"pageSize":20,"totalPages":0,"totalRecords":0}
```

### Description

The scanner detected a vulnerability on the web server side. It stems from the fact that responses received by the endpoints do not contain the `X-Content-Type-Options` security header.

### Risk

In environments where MIME sniffing is permitted, browsers may interpret content differently from the declared `Content-Type`, increasing the impact of content injection vulnerabilities.

### Remedeation

To resolve this, you need to configure the server to send the `X-Content-Type-Options` header with the `nosniff` directive. The header instructs browsers to respect the declared `Content-Type` and disables MIME sniffing.

#### Implementaion in project

1. Create a special middleware for security headers:
```cs
public class SecurityHeadersMiddleware
{
    ...
     public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        await _next(context);
    }
}
```

2. Move middleware registration to ApiExtensions:
```cs
public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
{
    return app.UseMiddleware<SecurityHeadersMiddleware>();
}
```

3. Include in Program.cs:
```cs
app.UseSecurityHeaders(); 
```

### Verifiaction

Following the implementation of security measures, a re-scan was conducted using ZAP.

No alerts regarding this vulnerability were detected after the scan was run.

We can also verify this by sending a GET request to the same URL via Burp or Scalar.

Requsest:
```HTTP
GET /api/transactions HTTP/1.1
Host: localhost:9090
authorization: Bearer <token>
User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)
```

Response:
```HTTP
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Wed, 08 Jul 2026 20:51:16 GMT
Server: Kestrel
X-Content-Type-Options: nosniff
Content-Length: 73
```

As we can see, the `X-Content-Type-Options` header has appeared, confirming that the vulnerability has been remediated.

---

### DAST-02 – Authentication Request Identified

#### Tool
`OWASP ZAP`

#### Type
`Informational`

#### Confidience
`High`

#### Event Details

URL: `http://localhost:9090/api/auth/login`

Method: `POST`

Response:
```HTTP
POST http://localhost:9090/api/auth/login HTTP/1.1
Host: localhost:9090
Content-Type: application/json
Content-Length: 48

{"email":"zaproxy@example.com","password":"ZAP"}

```

Request:
```HTTP
HTTP/1.1 401 Unauthorized
Content-Type: application/json; charset=utf-8
Server: Kestrel

{"message":"Invalid email or password."}

```

### Description

Informational finding. The scanner successfully identified the system login endpoint and mapped the parameters for transmitting the login email and password.

### Risk

There are no risks. This alert does not indicate a vulnerability.

### Remediation

No remediation required.

---

## False Positives

Analysis of the vulnerability scan results revealed no false positives.

The detected issues were manually verified to distinguish actual security problems from false positives.

---

## Security Assessment

Key web application endpoints were analyzed during automated Dynamic Application Security Testing (DAST).

Based on the performed assessment, no critical or high-risk vulnerabilities were identified. One low-risk security misconfiguration was remediated and successfully verified.

Vulnerability DAST-01, related to a security header, has been remediated and re-verified.

DAST-02 is an informational scanner observation indicating that OWASP ZAP successfully recognized the authentication workflow. No security issue was identified.

---

## Future Improvements

* ZAP integration into GitHub Actions
* Authenticated DAST
* Auditing and Alerting