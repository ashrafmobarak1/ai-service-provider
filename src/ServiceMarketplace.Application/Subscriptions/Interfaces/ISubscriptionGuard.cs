using ServiceMarketplace.Application.Common;

namespace ServiceMarketplace.Application.Subscriptions.Interfaces;

/// <summary>
/// Enforces subscription limits before allowing feature usage.
/// Called by services — not by controllers — to keep the guard close to business logic.
/// </summary>
public interface ISubscriptionGuard
{
    /// <summary>
    /// Returns Failure if the user has reached their plan's request limit.
    /// Returns Success if the action is allowed.
    /// </summary>
    Task<Result> CheckRequestCreationAllowedAsync(Guid userId);
}
