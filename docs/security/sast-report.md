# SAST Security Report

## Purpose

This document presents the results of an analysis conducted using SAST scanners. The objective was to identify potential security issues in the application source code and deployment artifacts before deployment.

---

## Methodology

The analysis was conducted using automated Static Application Security Testing (SAST).

All identified vulnerabilities were analyzed and manually verified to distinguish them from false positives.

### Lifecycle

```Text
Scan
↓
Review and assessment
↓
Mitigations
↓
Verification of corrections
↓
Documentation
```

---

## Tools

* Semgrep (C# Rules) – Security analysis of C# source code
* Semgrep (Auto Ruleset) – Multi-language security analysis
* .NET Analyzers – Static code quality and Roslyn-based diagnostics

---

## Summary

**Overall scanner result**

| Tool | Findings |
| --- | --- |
| Semgrep (C#) | 0 |
| Semgrep (Auto) | 1 |
| .NET Analyzers | 0 |

**Findings**

| ID | Rule | Severity | Status |
| --- | --- | --- | --- |
| SAST-01 | Dockerfile runs as root | Medium | Fixed |

---

## Findings

### SAST-01 – Container executed as root user

#### Tool 
`Semgrep (Auto option)`

#### Severity
`Medium`

#### CWE
```Text
CWE-269: Improper Privilege Management
```

#### OWASP
```Text
OWASP Top 10 2021: A04 — Insecure Design
OWASP Top 10 2025: A06- Insecure Design
```

### Description

A vulnerability in the Dockerfile was identified by the scanner.

It stemmed from the fact that the Docker container was configured to run as the root user by default, as the `USER` directive was not explicitly specified in the Dockerfile.

Semgrep message:
```Text
By not specifying a USER, a program in the container may run as 'root'.
This is a security hazard. If an attacker can control a process running as root, they may have control over the container.
Ensure that the last USER in a Dockerfile is a USER other than 'root'.
```

### Risk

Running the application as the root user increases the impact of a successful container compromise. An attacker could modify application files, access sensitive resources within the container, or attempt further privilege escalation, violating the Principle of Least Privilege.

### Mitigations

Creating a non-root user and assigning ownership of the application files to that user helped reduce risk. Now, the Dockerfile will launch the application as `appuser`.

Implementation in Dockerfile:
```Dockerfile
RUN adduser \
    --disabled-password \
    --home /app \
    appuser

COPY --chown=appuser:appuser --from=build /app/publish .

USER appuser
```

### Verification

Verification of successful vulnerability remediation:

1. Stop running Docker containers and start rebuilding the images..
```Bash
docker compose down
docker compose up -d --build
```
2. Verify that the application has successfully started up.
```Bash
...
✔ Container financetracker-db       Started                                               
✔ Container financetracker-api      Started
```
3. Checking the user and ID inside the Docker container.
```Bash
docker exec -it financetracker-api sh
$ whoami
appuser
$ id
uid=1000(appuser) gid=1000(appuser) groups=1000(appuser),100(users)
```
4. Relaunching the Semgrep scanner with the `auto` parameter.
```Bash
semgrep scan --config=auto
```
5. Verify that no findings are found.
```Bash
┌──────────────┐
│ Scan Summary │
└──────────────┘
✅ Scan completed successfully.
 • Findings: 0 (0 blocking)
```

---

## False Positives

Analysis of the vulnerability scan results revealed no false positives.

The detected issues were manually verified to distinguish actual security problems from false positives.

---

## Conclusion

The assessment identified one infrastructure-related security finding affecting the Docker runtime configuration.

The issue was successfully remediated by introducing a dedicated non-privileged application user and verified through repeated automated scanning.

No security findings were reported by Semgrep (C# Rules) or .NET Analyzers after remediation.

Overall, the application demonstrated a strong security posture with no remaining issues detected by the performed SAST tools.

#### Inspection results

Semgrep (C# option):
```Bash
semgrep scan --config=p/csharp
┌──────────────┐
│ Scan Summary │
└──────────────┘
✅ Scan completed successfully.
 • Findings: 0 (0 blocking)
 • Rules run: 27
```

Semgrep (Auto option):
```Bash
semgrep scan --config=auto
┌──────────────┐
│ Scan Summary │
└──────────────┘
✅ Scan completed successfully.
 • Findings: 0 (0 blocking)
 • Rules run: 122
```

.NET Analyzers:
Options in .csproj:
```csproj
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisLevel>latest</AnalysisLevel>
```
Result:
```Bash
dotnet build
```
The application built successfully.

---

## Future Improvments

* Semgrep integration into GitHub Actions
* Dependency vulnerability scanning
* Secrets detection (Gitleaks)
* SARIF reporting
* Custom Semgrep rules
