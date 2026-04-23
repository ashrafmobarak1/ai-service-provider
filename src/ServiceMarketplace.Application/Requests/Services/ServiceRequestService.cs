using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.Requests.DTOs;
using ServiceMarketplace.Application.Requests.Interfaces;
using ServiceMarketplace.Application.Subscriptions.Interfaces;

namespace ServiceMarketplace.Application.Requests.Services;

public class ServiceRequestService : IServiceRequestService
{
    private readonly IServiceRequestRepository _requestRepository;
    private readonly ISubscriptionGuard _subscriptionGuard;

    public ServiceRequestService(
        IServiceRequestRepository requestRepository,
        ISubscriptionGuard subscriptionGuard)
    {
        _requestRepository = requestRepository;
        _subscriptionGuard = subscriptionGuard;
    }

    public Task<Result<ServiceRequestDto>> CreateAsync(CreateRequestDto dto, Guid customerId)
        => throw new NotImplementedException();

    public Task<Result<IEnumerable<ServiceRequestDto>>> GetMyRequestsAsync(Guid customerId)
        => throw new NotImplementedException();

    public Task<Result<IEnumerable<ServiceRequestDto>>> GetAllPendingAsync()
        => throw new NotImplementedException();

    public Task<Result<IEnumerable<ServiceRequestDto>>> GetNearbyAsync(NearbyRequestsQuery query)
        => throw new NotImplementedException();

    public Task<Result<ServiceRequestDto>> AcceptAsync(Guid requestId, Guid providerId)
        => throw new NotImplementedException();

    public Task<Result<ServiceRequestDto>> CompleteAsync(Guid requestId, Guid providerId)
        => throw new NotImplementedException();

    public Task<Result> CancelAsync(Guid requestId, Guid customerId)
        => throw new NotImplementedException();
}
