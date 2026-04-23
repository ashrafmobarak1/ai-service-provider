using ServiceMarketplace.Application.AI.DTOs;
using ServiceMarketplace.Application.Common;

namespace ServiceMarketplace.Application.AI.Interfaces;

public interface IAIService
{
    /// <summary>
    /// Sends the request title and description to the AI model and returns
    /// a cleaner, more professional version of the description.
    /// </summary>
    Task<Result<EnhanceDescriptionDto>> EnhanceDescriptionAsync(Guid requestId);
}
