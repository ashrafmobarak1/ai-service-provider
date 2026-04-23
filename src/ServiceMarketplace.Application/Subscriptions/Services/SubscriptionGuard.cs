using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.RBAC.Interfaces;
using ServiceMarketplace.Application.Requests.Interfaces;
using ServiceMarketplace.Application.Subscriptions.Interfaces;
using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Application.Subscriptions.Services;

public class SubscriptionGuard : ISubscriptionGuard
{
    private readonly IUserRepository _userRepository;
    private readonly IServiceRequestRepository _requestRepository;

    // Free tier limit — could be moved to config/DB for flexibility
    private const int FreeTierMaxRequests = 3;

    public SubscriptionGuard(
        IUserRepository userRepository,
        IServiceRequestRepository requestRepository)
    {
        _userRepository = userRepository;
        _requestRepository = requestRepository;
    }

    public Task<Result> CheckRequestCreationAllowedAsync(Guid userId)
        => throw new NotImplementedException();
}
