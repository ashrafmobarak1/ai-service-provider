using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.Subscriptions.DTOs;

namespace ServiceMarketplace.Application.Subscriptions.Interfaces;

public interface ISubscriptionService
{
    Task<Result<SubscriptionDto>> GetStatusAsync(Guid userId);

    Task<Result<SubscriptionDto>> UpgradeAsync(Guid userId, UpgradeSubscriptionDto dto);
}
