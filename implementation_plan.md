# Implementation Plan - Core Business Logic

Implement the `NotImplementedException` stubs across the core service layers to provide functional Authentication, Service Request management, RBAC, Subscription limits, and AI-driven description enhancement.

## User Review Required

> [!IMPORTANT]
> The implementation assumes standard business rules for an MVP (e.g., users default to the 'Customer' role upon registration).
> AI enhancement is non-blocking — if the AI service fails, the core request creation still succeeds (as noted in the README).

## Proposed Changes

### [Auth]

#### [MODIFY] [AuthService.cs](file:///e:/projects/ai-service-provider/src/ServiceMarketplace.Application/Auth/Services/AuthService.cs)
- Implement `RegisterAsync`:
  - Validate that the email is not already in use.
  - Hash the password using `IPasswordHasher`.
  - Create the `User` entity.
  - Assign the default "Customer" role.
  - Generate and return a JWT token.
- Implement `LoginAsync`:
  - Fetch user by email.
  - Verify password hash.
  - Generate and return a JWT token.

---

### [Service Requests]

#### [MODIFY] [ServiceRequestService.cs](file:///e:/projects/ai-service-provider/src/ServiceMarketplace.Application/Requests/Services/ServiceRequestService.cs)
- Implement `CreateAsync`:
  - Call `ISubscriptionGuard` to verify if the user is allowed to create more requests.
  - Map DTO to `ServiceRequest` entity.
  - Persist via `IServiceRequestRepository`.
- Implement `GetMyRequestsAsync`: Fetch requests for the authenticated customer.
- Implement `GetAllPendingAsync`: Fetch all requests with 'Pending' status.
- Implement `GetNearbyAsync`: Delegate to repository for spatial lookup (using application-layer Haversine as per README).
- Implement `AcceptAsync`: Update status to 'Accepted', assign provider, and set timestamp.
- Implement `CompleteAsync`: Update status to 'Completed' and set timestamp.
- Implement `CancelAsync`: Update status to 'Cancelled'.

---

### [RBAC]

#### [MODIFY] [PermissionService.cs](file:///e:/projects/ai-service-provider/src/ServiceMarketplace.Application/RBAC/Services/PermissionService.cs)
- Implement `HasPermissionAsync`:
  - Fetch effective permissions from `IPermissionRepository`.
  - Check if the required permission name exists in the set.

---

### [Subscriptions]

#### [MODIFY] [SubscriptionGuard.cs](file:///e:/projects/ai-service-provider/src/ServiceMarketplace.Application/Subscriptions/Services/SubscriptionGuard.cs)
- Implement `CheckRequestCreationAllowedAsync`:
  - If user is `Paid`, always allow.
  - If user is `Free`, count active/pending requests.
  - Return failure if the count exceeds `FreeTierMaxRequests` (3).

---

### [AI Enhancement]

#### [MODIFY] [AIService.cs](file:///e:/projects/ai-service-provider/src/ServiceMarketplace.Application/AI/Services/AIService.cs)
- Implement `EnhanceDescriptionAsync`:
  - Fetch request by ID.
  - Call `IAIGateway.EnhanceTextAsync`.
  - Update `AiDescription` and persist.

## Verification Plan

### Automated Tests
- Run `dotnet test` to verify business logic if unit tests exist.

### Manual Verification
- **Swagger/Postman**: 
  - Register a new user.
  - Login to get a token.
  - Create service requests.
  - Try to exceed the 3-request limit on a free account.
  - Accept a request as a provider.
  - Verify AI enhancement updates the database.
