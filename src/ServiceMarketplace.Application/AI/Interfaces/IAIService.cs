using ServiceMarketplace.Application.AI.DTOs;
using ServiceMarketplace.Application.Common;

namespace ServiceMarketplace.Application.AI.Interfaces;

public interface IAIService
{
    /// <summary>
    /// Enqueues a background job to enhance the description.
    /// Returns the Hangfire JobId.
    /// </summary>
    Task<Result<string>> EnqueueEnhancementJobAsync(Guid requestId);

    /// <summary>
    /// The actual logic executed by the background worker.
    /// </summary>
    Task ProcessEnhancementJobAsync(Guid requestId);
}
