using ServiceMarketplace.Application.AI.DTOs;
using ServiceMarketplace.Application.AI.Interfaces;
using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.Requests.Interfaces;
using ServiceMarketplace.Domain.Entities;

namespace ServiceMarketplace.Application.AI.Services;

public class AIService : IAIService
{
    private readonly IServiceRequestRepository _requestRepository;
    private readonly IAIGateway _aiGateway;

    public AIService(
        IServiceRequestRepository requestRepository,
        IAIGateway aiGateway)
    {
        _requestRepository = requestRepository;
        _aiGateway = aiGateway;
    }

    public async Task<Result<EnhanceDescriptionDto>> EnhanceDescriptionAsync(Guid requestId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null)
            return Result<EnhanceDescriptionDto>.Failure("Service request not found.");

        try
        {
            var enhancedText = await _aiGateway.EnhanceTextAsync(request.Title, request.Description);
            
            request.AiDescription = enhancedText;
            request.UpdatedAt = DateTime.UtcNow;
            
            await _requestRepository.UpdateAsync(request);

            return Result<EnhanceDescriptionDto>.Success(new EnhanceDescriptionDto
            {
                OriginalTitle = request.Title,
                OriginalDescription = request.Description,
                EnhancedDescription = enhancedText
            });
        }
        catch (Exception ex)
        {
            // Logging would go here in a real app
            return Result<EnhanceDescriptionDto>.Failure($"AI enhancement failed: {ex.Message}. The core request remains valid.");
        }
    }
}
