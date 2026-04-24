using ServiceMarketplace.Application.AI.DTOs;
using ServiceMarketplace.Application.AI.Interfaces;
using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.Requests.Interfaces;
using ServiceMarketplace.Domain.Entities;
using Hangfire;

namespace ServiceMarketplace.Application.AI.Services;

public class AIService : IAIService
{
    private readonly IServiceRequestRepository _requestRepository;
    private readonly IAIGateway _aiGateway;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public AIService(
        IServiceRequestRepository requestRepository,
        IAIGateway aiGateway,
        IBackgroundJobClient backgroundJobClient)
    {
        _requestRepository = requestRepository;
        _aiGateway = aiGateway;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Result<string>> EnqueueEnhancementJobAsync(Guid requestId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null)
            return Result<string>.Failure("Service request not found.");

        var jobId = _backgroundJobClient.Enqueue<IAIService>(x => x.ProcessEnhancementJobAsync(requestId));
        return Result<string>.Success(jobId);
    }

    public async Task ProcessEnhancementJobAsync(Guid requestId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null) return;

        try
        {
            var enhancedText = await _aiGateway.EnhanceTextAsync(request.Title, request.Description);
            
            request.AiDescription = enhancedText;
            request.UpdatedAt = DateTime.UtcNow;
            
            await _requestRepository.UpdateAsync(request);
        }
        catch (Exception)
        {
            // In a real app, Hangfire will automatically retry based on the exception.
            throw; 
        }
    }
}
