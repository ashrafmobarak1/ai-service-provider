using Microsoft.Extensions.DependencyInjection;
using ServiceMarketplace.Application.AI.Interfaces;
using ServiceMarketplace.Application.AI.Services;
using ServiceMarketplace.Application.Auth.Interfaces;
using ServiceMarketplace.Application.Auth.Services;
using ServiceMarketplace.Application.RBAC.Interfaces;
using ServiceMarketplace.Application.RBAC.Services;
using ServiceMarketplace.Application.Requests.Interfaces;
using ServiceMarketplace.Application.Requests.Services;
using ServiceMarketplace.Application.Subscriptions.Interfaces;
using ServiceMarketplace.Application.Subscriptions.Services;

namespace ServiceMarketplace.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IServiceRequestService, ServiceRequestService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ISubscriptionGuard, SubscriptionGuard>();
        services.AddScoped<IAIService, AIService>();

        return services;
    }
}
