using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.RBAC.Interfaces;
using ServiceMarketplace.Application.Requests.Interfaces;
using ServiceMarketplace.Application.Subscriptions.DTOs;
using ServiceMarketplace.Application.Subscriptions.Interfaces;

namespace ServiceMarketplace.Application.Subscriptions.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IUserRepository _userRepository;
    private readonly IServiceRequestRepository _requestRepository;

    public SubscriptionService(
        IUserRepository userRepository,
        IServiceRequestRepository requestRepository)
    {
        _userRepository = userRepository;
        _requestRepository = requestRepository;
    }

    public Task<Result<SubscriptionDto>> GetStatusAsync(Guid userId)
        => throw new NotImplementedException();

    public Task<Result<SubscriptionDto>> UpgradeAsync(Guid userId, UpgradeSubscriptionDto dto)
        => throw new NotImplementedException();
}
