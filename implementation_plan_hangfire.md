# Implementation Plan - Background Jobs (Hangfire)

Move the AI enhancement logic from a blocking HTTP call to an asynchronous background job using Hangfire with MySQL storage.

## User Review Required

> [!NOTE]
> Hangfire will create its own tables in the `service_marketplace` database. 
> The API endpoint for AI enhancement will now return a `202 Accepted` status with a job ID, rather than the enhanced text immediately.

## Proposed Changes

### [Infrastructure]

#### [MODIFY] [InfrastructureExtensions.cs](file:///e:/projects/ai-service-provider/src/ServiceMarketplace.Infrastructure/InfrastructureExtensions.cs)
- Register Hangfire services.
- Configure `MySqlStorage` using the existing connection string.
- Add Hangfire server to the service collection.

#### [MODIFY] [Program.cs](file:///e:/projects/ai-service-provider/src/ServiceMarketplace.API/Program.cs)
- Add `app.UseHangfireDashboard()` to allow monitoring jobs in development.

---

### [Application]

#### [MODIFY] [AIService.cs](file:///e:/projects/ai-service-provider/src/ServiceMarketplace.Application/AI/Services/AIService.cs)
- Refactor `EnhanceDescriptionAsync` to use `IBackgroundJobClient` to enqueue a new method.
- Add a new method `ProcessEnhancementJobAsync(Guid requestId)` that contains the actual logic (calling the gateway and updating the DB).

---

### [API]

#### [MODIFY] [AIController.cs](file:///e:/projects/ai-service-provider/src/ServiceMarketplace.API/Controllers/AIController.cs)
- Update the `Enhance` endpoint to return a success message indicating the job has been queued.

## Verification Plan

### Automated Tests
- Build the project to ensure package compatibility.

### Manual Verification
- **Hangfire Dashboard**: Visit `http://localhost:5214/hangfire` to see the job queue.
- **API Test**: 
  - Call the enhancement endpoint.
  - Verify it returns immediately.
  - Refresh the database after a few seconds to see if `ai_description` was updated.
