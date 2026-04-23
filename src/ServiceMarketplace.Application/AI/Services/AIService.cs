using ServiceMarketplace.Application.AI.DTOs;
using ServiceMarketplace.Application.AI.Interfaces;
using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.Requests.Interfaces;

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

    public Task<Result<EnhanceDescriptionDto>> EnhanceDescriptionAsync(Guid requestId)
        => throw new NotImplementedException();
}
