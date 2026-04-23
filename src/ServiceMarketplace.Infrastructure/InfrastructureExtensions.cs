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
