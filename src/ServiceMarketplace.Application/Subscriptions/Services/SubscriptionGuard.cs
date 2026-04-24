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

    public async Task<Result> CheckRequestCreationAllowedAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result.Failure("User not found.");

        if (user.Subscription == SubscriptionTier.Paid)
            return Result.Success();

        // Free tier logic
        var activeRequestsCount = await _requestRepository.CountActiveByCustomerAsync(userId);
        if (activeRequestsCount >= FreeTierMaxRequests)
        {
            return Result.Failure($"Free tier limit reached. Maximum of {FreeTierMaxRequests} active requests allowed. Please upgrade to Paid tier for unlimited requests.");
        }

        return Result.Success();
    }
}
