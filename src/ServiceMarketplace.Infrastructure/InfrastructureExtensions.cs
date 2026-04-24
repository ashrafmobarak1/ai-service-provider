using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceMarketplace.Application.AI.Interfaces;
using ServiceMarketplace.Application.Auth.Interfaces;
using ServiceMarketplace.Application.RBAC.Interfaces;
using ServiceMarketplace.Application.Requests.Interfaces;
using ServiceMarketplace.Infrastructure.AI;
using ServiceMarketplace.Infrastructure.Identity;
using ServiceMarketplace.Infrastructure.Persistence;
using ServiceMarketplace.Infrastructure.Persistence.Repositories;
using Hangfire;
using Hangfire.MySql;
using System.Transactions;

namespace ServiceMarketplace.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Database ─────────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)));
        });

        // ── Hangfire ─────────────────────────────────────────────────────────
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(new MySqlStorage(configuration.GetConnectionString("DefaultConnection"), new MySqlStorageOptions
            {
                TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                QueuePollInterval = TimeSpan.FromSeconds(15),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true,
            })));

        services.AddHangfireServer();

        // ── Repositories ─────────────────────────────────────────────────────
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

        // ── Identity ──────────────────────────────────────────────────────────
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // ── AI Gateway ────────────────────────────────────────────────────────
        services.AddHttpClient<IAIGateway, ClaudeAIGateway>(client =>
        {
            client.BaseAddress = new Uri(configuration["AI:BaseUrl"] ?? "https://api.anthropic.com");
            client.DefaultRequestHeaders.Add("x-api-key", configuration["AI:ApiKey"]);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        });

        return services;
    }
}
