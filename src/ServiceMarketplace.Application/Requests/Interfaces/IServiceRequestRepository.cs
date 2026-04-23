using ServiceMarketplace.Application.Requests.DTOs;
using ServiceMarketplace.Domain.Entities;
using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Application.Requests.Interfaces;

public interface IServiceRequestRepository
{
    Task<ServiceRequest?> GetByIdAsync(Guid id);

    Task<IEnumerable<ServiceRequest>> GetByCustomerIdAsync(Guid customerId);

    Task<IEnumerable<ServiceRequest>> GetAllPendingAsync();

    Task<IEnumerable<(ServiceRequest Request, double DistanceKm)>> GetNearbyAsync(
        double latitude, double longitude, double radiusKm);

    Task<int> CountActiveByCustomerAsync(Guid customerId);

    Task AddAsync(ServiceRequest request);

    Task UpdateAsync(ServiceRequest request);
}
