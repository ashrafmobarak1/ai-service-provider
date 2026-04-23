using Microsoft.EntityFrameworkCore;
using ServiceMarketplace.Application.Requests.Interfaces;
using ServiceMarketplace.Domain.Entities;
using ServiceMarketplace.Domain.Enums;
using ServiceMarketplace.Infrastructure.Persistence;

namespace ServiceMarketplace.Infrastructure.Persistence.Repositories;

public class ServiceRequestRepository : IServiceRequestRepository
{
    private readonly AppDbContext _context;

    public ServiceRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<ServiceRequest?> GetByIdAsync(Guid id)
        => _context.ServiceRequests
            .Include(r => r.Customer)
            .Include(r => r.Provider)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<ServiceRequest>> GetByCustomerIdAsync(Guid customerId)
        => await _context.ServiceRequests
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<ServiceRequest>> GetAllPendingAsync()
        => await _context.ServiceRequests
            .Include(r => r.Customer)
            .Where(r => r.Status == RequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    /// <summary>
    /// Nearby query using the Haversine formula in-memory after a status filter.
    /// For production scale, replace with PostGIS ST_DWithin.
    /// </summary>
    public async Task<IEnumerable<(ServiceRequest Request, double DistanceKm)>> GetNearbyAsync(
        double latitude, double longitude, double radiusKm)
    {
        var pending = await _context.ServiceRequests
            .Include(r => r.Customer)
            .Where(r => r.Status == RequestStatus.Pending)
            .ToListAsync();

        return pending
            .Select(r => (Request: r, DistanceKm: Haversine(latitude, longitude, r.Latitude, r.Longitude)))
            .Where(x => x.DistanceKm <= radiusKm)
            .OrderBy(x => x.DistanceKm);
    }

    public Task<int> CountActiveByCustomerAsync(Guid customerId)
        => _context.ServiceRequests.CountAsync(r =>
            r.CustomerId == customerId &&
            (r.Status == RequestStatus.Pending || r.Status == RequestStatus.Accepted));

    public async Task AddAsync(ServiceRequest request)
    {
        await _context.ServiceRequests.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServiceRequest request)
    {
        request.UpdatedAt = DateTime.UtcNow;
        _context.ServiceRequests.Update(request);
        await _context.SaveChangesAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth radius in km
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double degrees) => degrees * Math.PI / 180;
}
