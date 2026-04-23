# Service Marketplace Platform — MVP

A fullstack service marketplace where customers create service requests and providers accept them nearby.
Built with **ASP.NET Core 8 Web API** + **PostgreSQL** + **React** (frontend, separate).

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Tech Stack](#2-tech-stack)
3. [Architecture Overview](#3-architecture-overview)
4. [Folder Structure](#4-folder-structure)
5. [Database Schema](#5-database-schema)
6. [RBAC Design](#6-rbac-design)
7. [Subscription / Feature Gating](#7-subscription--feature-gating)
8. [Geolocation](#8-geolocation)
9. [AI Feature](#9-ai-feature)
10. [API Reference](#10-api-reference)
11. [Setup Instructions](#11-setup-instructions)
12. [Running with Docker](#12-running-with-docker)
13. [Running Locally (without Docker)](#13-running-locally-without-docker)
14. [Environment Variables](#14-environment-variables)
15. [Key Design Decisions & Trade-offs](#15-key-design-decisions--trade-offs)
16. [Assumptions](#16-assumptions)
17. [What I Would Improve with More Time](#17-what-i-would-improve-with-more-time)

---

## 1. Project Overview

| Role | What they can do |
|---|---|
| **Customer** | Register, create service requests, view own requests |
| **Provider** | View all pending requests, filter nearby, accept & complete |
| **ProviderAdmin** | All Provider actions + manage permissions for their team |
| **Admin** | Full access — manage users, roles, permissions |

**Request lifecycle:** `pending → accepted → completed`

---

## 2. Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL 16 |
| Auth | JWT Bearer tokens |
| Password | BCrypt |
| AI Feature | Claude API (claude-haiku-4-5) |
| Documentation | Swagger / OpenAPI |
| Containerization | Docker + Docker Compose |
| Frontend | React + TypeScript (Vite) |

---

## 3. Architecture Overview

The backend follows a **layered clean architecture** with strict dependency direction:

```
API  →  Application  →  Domain
         ↓
    Infrastructure  →  Domain
```

### Layers

**Domain** — Pure C# entities, enums, domain exceptions. Zero external dependencies.

**Application** — Business logic, use-case services, repository interfaces, DTOs.
Does not know about EF Core, HTTP, or any external provider.

**Infrastructure** — EF Core DbContext, repository implementations, JWT service,
password hasher, Claude AI gateway. Implements Application interfaces.

**API** — Controllers, middleware, permission filters, Swagger config, `Program.cs`.
Maps HTTP ↔ Application. No business logic here.

### Data Flow (example: Customer creates a request)

```
POST /api/requests
      │
      ▼
[JWT Middleware]        → validates token, populates ICurrentUser
      │
      ▼
[PermissionFilter]      → checks "request.create" permission for this user
      │
      ▼
[ServiceRequestController]
      │  maps request body → CreateRequestCommand
      ▼
[ServiceRequestService]
      │  checks subscription limit via ISubscriptionGuard
      │  calls IServiceRequestRepository.AddAsync(entity)
      │  optionally calls IAIService.EnhanceDescriptionAsync(...)
      ▼
[ServiceRequestRepository]   → EF Core → PostgreSQL
      │
      ▼
Result<ServiceRequestDto>    → 201 Created
```

---

## 4. Folder Structure

```
ServiceMarketplace/
├── src/
│   ├── ServiceMarketplace.Domain/
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Role.cs
│   │   │   ├── Permission.cs
│   │   │   ├── RolePermission.cs
│   │   │   ├── UserRole.cs
│   │   │   ├── UserPermission.cs
│   │   │   └── ServiceRequest.cs
│   │   ├── Enums/
│   │   │   ├── RequestStatus.cs
│   │   │   └── SubscriptionTier.cs
│   │   └── Exceptions/
│   │       ├── DomainException.cs
│   │       ├── UnauthorizedException.cs
│   │       └── SubscriptionLimitException.cs
│   │
│   ├── ServiceMarketplace.Application/
│   │   ├── Common/
│   │   │   ├── Result.cs                    ← Result<T> pattern
│   │   │   └── Interfaces/
│   │   │       ├── ICurrentUser.cs
│   │   │       └── IUnitOfWork.cs
│   │   ├── Auth/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IAuthService.cs
│   │   │   │   └── IJwtService.cs
│   │   │   ├── Services/AuthService.cs
│   │   │   └── DTOs/
│   │   │       ├── RegisterDto.cs
│   │   │       ├── LoginDto.cs
│   │   │       └── TokenDto.cs
│   │   ├── Requests/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IServiceRequestService.cs
│   │   │   │   └── IServiceRequestRepository.cs
│   │   │   ├── Services/ServiceRequestService.cs
│   │   │   └── DTOs/
│   │   │       ├── CreateRequestDto.cs
│   │   │       ├── ServiceRequestDto.cs
│   │   │       └── NearbyRequestsQuery.cs
│   │   ├── RBAC/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IPermissionService.cs
│   │   │   │   └── IRoleRepository.cs
│   │   │   ├── Services/PermissionService.cs
│   │   │   └── DTOs/
│   │   │       ├── RoleDto.cs
│   │   │       └── PermissionDto.cs
│   │   ├── Subscriptions/
│   │   │   ├── Interfaces/
│   │   │   │   ├── ISubscriptionService.cs
│   │   │   │   └── ISubscriptionGuard.cs
│   │   │   └── Services/
│   │   │       ├── SubscriptionService.cs
│   │   │       └── SubscriptionGuard.cs
│   │   └── AI/
│   │       ├── Interfaces/IAIService.cs
│   │       └── Services/AIService.cs
│   │
│   ├── ServiceMarketplace.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   ├── ServiceRequestConfiguration.cs
│   │   │   │   └── RoleConfiguration.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── UserRepository.cs
│   │   │   │   ├── ServiceRequestRepository.cs
│   │   │   │   ├── PermissionRepository.cs
│   │   │   │   └── RoleRepository.cs
│   │   │   └── Migrations/
│   │   ├── Identity/
│   │   │   ├── JwtService.cs
│   │   │   └── PasswordHasher.cs
│   │   ├── AI/
│   │   │   └── ClaudeAIGateway.cs
│   │   └── InfrastructureExtensions.cs
│   │
│   └── ServiceMarketplace.API/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── ServiceRequestController.cs
│       │   ├── SubscriptionController.cs
│       │   ├── AdminController.cs
│       │   └── AIController.cs
│       ├── Middleware/
│       │   └── ExceptionHandlingMiddleware.cs
│       ├── Attributes/
│       │   └── RequiresPermissionAttribute.cs
│       ├── Filters/
│       │   └── PermissionAuthorizationFilter.cs
│       ├── Services/
│       │   └── CurrentUserService.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── Program.cs
│
├── tests/
│   ├── ServiceMarketplace.UnitTests/
│   └── ServiceMarketplace.IntegrationTests/
│
├── docker-compose.yml
├── Dockerfile
└── README.md
```

---

## 5. Database Schema

### Tables

```
users
─────────────────────────────────────────────────────
id              UUID            PK
name            VARCHAR(100)    NOT NULL
email           VARCHAR(255)    NOT NULL  UNIQUE
password_hash   VARCHAR(255)    NOT NULL
subscription    ENUM            'free' | 'paid'   DEFAULT 'free'
is_active       BOOLEAN         DEFAULT true
created_at      TIMESTAMPTZ
updated_at      TIMESTAMPTZ


roles
─────────────────────────────────────────────────────
id              UUID            PK
name            VARCHAR(50)     NOT NULL  UNIQUE
description     VARCHAR(255)
parent_role_id  UUID            FK → roles.id   (self-reference for hierarchy)
created_at      TIMESTAMPTZ


permissions
─────────────────────────────────────────────────────
id          UUID            PK
name        VARCHAR(100)    NOT NULL  UNIQUE   e.g. "request.create"
resource    VARCHAR(50)                        e.g. "request"
action      VARCHAR(50)                        e.g. "create"
description VARCHAR(255)
created_at  TIMESTAMPTZ


role_permissions
─────────────────────────────────────────────────────
role_id         UUID    PK + FK → roles.id       CASCADE DELETE
permission_id   UUID    PK + FK → permissions.id CASCADE DELETE
created_at      TIMESTAMPTZ


user_roles
─────────────────────────────────────────────────────
user_id     UUID    PK + FK → users.id    CASCADE DELETE
role_id     UUID    PK + FK → roles.id    CASCADE DELETE
assigned_by UUID    FK → users.id         SET NULL
assigned_at TIMESTAMPTZ


user_permissions   ← direct grants or explicit denies
─────────────────────────────────────────────────────
user_id         UUID    PK + FK → users.id
permission_id   UUID    PK + FK → permissions.id
is_granted      BOOLEAN          true = grant, false = explicit deny
granted_by      UUID    FK → users.id
granted_at      TIMESTAMPTZ


service_requests
─────────────────────────────────────────────────────
id              UUID            PK
title           VARCHAR(200)    NOT NULL
description     TEXT            NOT NULL
ai_description  TEXT            nullable — set after AI enhancement
status          ENUM            'pending' | 'accepted' | 'completed' | 'cancelled'
latitude        DOUBLE PRECISION NOT NULL
longitude       DOUBLE PRECISION NOT NULL
address         VARCHAR(500)
customer_id     UUID    FK → users.id    RESTRICT
provider_id     UUID    FK → users.id    SET NULL (null until accepted)
accepted_at     TIMESTAMPTZ
completed_at    TIMESTAMPTZ
created_at      TIMESTAMPTZ
updated_at      TIMESTAMPTZ


subscription_plans   ← reference / config table
─────────────────────────────────────────────────────
id              UUID            PK
tier            ENUM            UNIQUE
max_requests    INT             nullable = unlimited
description     VARCHAR(255)
```

### Relationships

```
users  1──M  service_requests (as customer)
users  1──M  service_requests (as provider, optional)
users  M──M  roles             via user_roles
roles  M──M  permissions       via role_permissions
users  M──M  permissions       via user_permissions (direct override)
roles  1──M  roles             parent_role_id (self-reference)
```

---

## 6. RBAC Design

### How Permissions Are Stored

Permissions are **string keys** stored in the `permissions` table — not hardcoded enums.

```
request.create    request.view_own   request.view_all
request.accept    request.complete   request.cancel
user.manage       role.manage
```

### Default Role → Permission Mapping (seeded at startup)

| Role | Permissions |
|---|---|
| Admin | All |
| ProviderAdmin | view_all, accept, complete, role.manage |
| Provider | view_all, accept, complete |
| Customer | create, view_own, cancel |

### Permission Resolution (evaluated at every request)

```
1. Check user_permissions WHERE is_granted = false  →  DENY  (explicit deny wins)
2. Check user_permissions WHERE is_granted = true   →  GRANT (direct grant)
3. Check role_permissions via user_roles             →  GRANT (role default)
4. Default                                           →  DENY
```

### Enforcement at API Level

```csharp
// Attribute placed on controller action
[RequiresPermission("request.create")]
public async Task<IActionResult> Create([FromBody] CreateRequestDto dto) { ... }

// PermissionAuthorizationFilter reads the attribute,
// calls IPermissionService.HasPermissionAsync(userId, "request.create"),
// returns 403 Forbidden if false — before the controller body executes.
```

### Role Hierarchy

`ProviderAdmin` has `parent_role_id = null` but `Provider.parent_role_id = ProviderAdmin.id`.
This allows a ProviderAdmin to grant/revoke Provider-level permissions to users who share
the same parent role — enforced in `PermissionService`, not at DB level.

### Dynamic Assignment (Admin API)

```
POST   /api/admin/roles/{roleId}/permissions/{permissionId}   → grant to role
DELETE /api/admin/roles/{roleId}/permissions/{permissionId}   → revoke from role
POST   /api/admin/users/{userId}/permissions/{permissionId}   → direct grant to user
DELETE /api/admin/users/{userId}/permissions/{permissionId}   → direct deny/revoke
POST   /api/admin/users/{userId}/roles/{roleId}               → assign role
```

---

## 7. Subscription / Feature Gating

The `users.subscription` column holds `'free'` or `'paid'`.

| Tier | Limit |
|---|---|
| Free | Max **3** active (pending/accepted) service requests |
| Paid | Unlimited |

The guard runs **before** inserting a new request:

```csharp
// ISubscriptionGuard
var count = await _repo.CountActiveRequestsByCustomer(customerId);
var plan  = await _planRepo.GetByTier(user.Subscription);

if (plan.MaxRequests.HasValue && count >= plan.MaxRequests.Value)
    return Result.Failure("Subscription limit reached. Upgrade to Paid.");
```

Upgrade is **simulated** — no real payment provider. A single endpoint sets the flag:

```
POST /api/subscriptions/upgrade
Body: { "tier": "paid" }
→ updates users.subscription = 'paid'
```

---

## 8. Geolocation

Each `ServiceRequest` stores `latitude` and `longitude` as `DOUBLE PRECISION`.

### Nearby Query — Haversine Formula

```sql
SELECT *,
  6371 * acos(
    cos(radians(:lat)) * cos(radians(latitude))
    * cos(radians(longitude) - radians(:lng))
    + sin(radians(:lat)) * sin(radians(latitude))
  ) AS distance_km
FROM service_requests
WHERE status = 'pending'
HAVING distance_km <= :radius_km
ORDER BY distance_km;
```

Exposed via:

```
GET /api/requests/nearby?lat=31.5&lng=34.8&radiusKm=10
```

Upgrade path if needed: add a `GEOGRAPHY(Point, 4326)` column and `ST_DWithin` index
(PostGIS) without redesigning the schema.

---

## 9. AI Feature

**Feature:** AI-enhanced service request description.

When a customer creates a request, they can optionally call:

```
POST /api/requests/{id}/enhance
```

This sends the original title + description to the **Claude API** (`claude-haiku-4-5`)
and returns a cleaner, more professional version — stored in `ai_description`.

**Prompt used:**

```
You are a service marketplace assistant.
Given a service request title and description, rewrite the description to be
clear, professional, and helpful for service providers.
Return only the improved description, no additional text.

Title: {title}
Description: {description}
```

The gateway (`ClaudeAIGateway`) wraps the Anthropic API using `HttpClient`.
If the API call fails, the original description is preserved — no disruption to the
core flow.

---

## 10. API Reference

Full interactive docs available at `/swagger` when running.

### Auth

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | None | Register new user |
| POST | `/api/auth/login` | None | Login, receive JWT |

### Service Requests

| Method | Endpoint | Permission | Description |
|---|---|---|---|
| POST | `/api/requests` | request.create | Customer creates request |
| GET | `/api/requests` | request.view_own | Customer views own requests |
| GET | `/api/requests/all` | request.view_all | Provider/Admin views all pending |
| GET | `/api/requests/nearby` | request.view_all | Provider gets nearby requests |
| PUT | `/api/requests/{id}/accept` | request.accept | Provider accepts request |
| PUT | `/api/requests/{id}/complete` | request.complete | Provider completes request |
| DELETE | `/api/requests/{id}` | request.cancel | Customer cancels pending request |

### AI

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/requests/{id}/enhance` | JWT | Enhance description with AI |

### Subscription

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/subscriptions/status` | JWT | View current subscription |
| POST | `/api/subscriptions/upgrade` | JWT | Simulate upgrade to paid |

### Admin

| Method | Endpoint | Permission | Description |
|---|---|---|---|
| GET | `/api/admin/roles` | role.manage | List all roles |
| POST | `/api/admin/roles/{roleId}/permissions/{permId}` | role.manage | Grant permission to role |
| DELETE | `/api/admin/roles/{roleId}/permissions/{permId}` | role.manage | Revoke from role |
| POST | `/api/admin/users/{userId}/roles/{roleId}` | user.manage | Assign role to user |
| POST | `/api/admin/users/{userId}/permissions/{permId}` | role.manage | Direct grant to user |
| DELETE | `/api/admin/users/{userId}/permissions/{permId}` | role.manage | Direct deny/revoke |

---

## 11. Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 16](https://www.postgresql.org/download/) (or use Docker)
- [Node.js 20+](https://nodejs.org/) (for frontend)

---

## 12. Running with Docker

This is the recommended approach — one command starts everything.

```bash
# Clone the repository
git clone <your-repo-url>
cd ServiceMarketplace

# Copy and configure environment
cp .env.example .env
# Edit .env — set CLAUDE_API_KEY at minimum

# Start all services (API + PostgreSQL)
docker compose up --build
```

| Service | URL |
|---|---|
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| PostgreSQL | localhost:5432 |

To stop:

```bash
docker compose down
# To also remove database volume:
docker compose down -v
```

---

## 13. Running Locally (without Docker)

### 1. PostgreSQL

Make sure PostgreSQL is running and create the database:

```sql
CREATE DATABASE service_marketplace;
```

### 2. Configure

Edit `src/ServiceMarketplace.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=service_marketplace;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "Issuer": "ServiceMarketplace",
    "Audience": "ServiceMarketplaceUsers",
    "ExpiryMinutes": 60
  },
  "AI": {
    "ApiKey": "your-claude-api-key",
    "Model": "claude-haiku-4-5-20251001",
    "BaseUrl": "https://api.anthropic.com"
  }
}
```

### 3. Apply Migrations

```bash
cd src/ServiceMarketplace.API
dotnet ef database update
```

This creates all tables and runs the seed data (roles, permissions, default assignments).

### 4. Run the API

```bash
dotnet run --project src/ServiceMarketplace.API
```

API runs at `http://localhost:5000`. Swagger at `http://localhost:5000/swagger`.

### 5. Default Seeded Admin Account

```
Email:    admin@marketplace.com
Password: Admin@123456
```

---

## 14. Environment Variables

| Variable | Description | Default |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | — |
| `Jwt__Key` | JWT signing secret (min 32 chars) | — |
| `Jwt__ExpiryMinutes` | Token lifetime in minutes | `60` |
| `AI__ApiKey` | Claude API key | — |
| `AI__Model` | Claude model ID | `claude-haiku-4-5-20251001` |

---

## 15. Key Design Decisions & Trade-offs

### Result\<T\> over exceptions for expected failures

Services return `Result<T>` (success/failure + message) instead of throwing exceptions.
Only truly unexpected errors bubble to `ExceptionHandlingMiddleware`.
Keeps controllers clean — no try/catch blocks in request handlers.

### Permission checked per-request, not cached

For MVP correctness, permission checks hit the DB on each request.
The query is a single indexed lookup across two small tables — fast enough at this scale.
Caching (Redis, 5-min TTL) is the natural next step if throughput demands it.

### Subscription stored as a column on `users`, not a separate table

Avoids a JOIN on every request creation guard check.
Trade-off: no subscription history. If billing records are needed, add a
`subscription_history` table alongside without changing this column.

### Haversine in application layer, not PostGIS

No external PostgreSQL extension required for MVP deployment.
The formula is accurate enough within the radius sizes likely used (< 50 km).
Trade-off: cannot use a spatial index, so nearby queries do a full table scan on
`service_requests WHERE status = 'pending'`. The partial index on `status = 'pending'`
limits the scan to only the relevant subset.

### No MediatR / CQRS for MVP

The assignment explicitly says avoid over-engineering. MediatR adds indirection
(Command → Handler pipeline) that is valuable at scale but adds boilerplate and
complexity that doesn't pay off here. Services are injected directly into controllers.
The structure is still CQRS-ready — adding handlers later is a mechanical refactor.

### AI feature is non-blocking

If the Claude API call fails (network error, rate limit), the request is still created
successfully with the original description. `ai_description` stays `null` until
explicitly enhanced. This keeps the core flow reliable.

---

## 16. Assumptions

- Only one active subscription tier per user at a time (no downgrade flow needed for MVP).
- "Active requests" for the subscription limit means `status IN ('pending', 'accepted')` —
  completed and cancelled requests do not count against the limit.
- A Provider can only accept requests they have not already accepted (one provider per request).
- ProviderAdmin can only manage permissions for users who share the `Provider` role —
  not arbitrary users. This scope check is enforced in `PermissionService`, not DB constraints.
- Email is case-insensitively unique (stored as-is, compared lowercased).
- JWT tokens are not revocable in MVP — logout is client-side only. A token blacklist
  (Redis) would be the production solution.

---

## 17. What I Would Improve with More Time

| Area | Improvement |
|---|---|
| **Geolocation** | Switch to PostGIS `ST_DWithin` with a spatial index for scalable nearby queries |
| **Caching** | Redis for permission checks and frequently read subscription plans |
| **Real-time** | SignalR hub to push request status changes to connected providers |
| **Auth** | Refresh token rotation, token revocation blacklist |
| **Background jobs** | Hangfire for async AI enhancement jobs (don't block the HTTP response) |
| **Testing** | Integration tests with Testcontainers (real PostgreSQL in CI) |
| **Payments** | Stripe Checkout integration replacing the simulated upgrade |
| **Observability** | Structured logging (Serilog + Seq), health checks, OpenTelemetry traces |
| **Rate limiting** | Per-user API rate limiting via ASP.NET Core rate limiter middleware |
| **CI/CD** | GitHub Actions pipeline: build → test → Docker push → deploy |
| **Audit log** | Separate `audit_log` table for permission changes and status transitions |
| **Multi-tenancy** | Organization/team concept for ProviderAdmin scoping at DB level |
