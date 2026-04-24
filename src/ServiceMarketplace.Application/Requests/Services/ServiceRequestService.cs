using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.Requests.DTOs;
using ServiceMarketplace.Application.Requests.Interfaces;
using ServiceMarketplace.Application.Subscriptions.Interfaces;
using ServiceMarketplace.Domain.Entities;
using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Application.Requests.Services;

public class ServiceRequestService : IServiceRequestService
{
    private readonly IServiceRequestRepository _requestRepository;
    private readonly ISubscriptionGuard _subscriptionGuard;

    public ServiceRequestService(
        IServiceRequestRepository requestRepository,
        ISubscriptionGuard subscriptionGuard)
    {
        _requestRepository = requestRepository;
        _subscriptionGuard = subscriptionGuard;
    }

    public async Task<Result<ServiceRequestDto>> CreateAsync(CreateRequestDto dto, Guid customerId)
    {
        var guardResult = await _subscriptionGuard.CheckRequestCreationAllowedAsync(customerId);
        if (!guardResult.IsSuccess)
            return Result<ServiceRequestDto>.Failure(guardResult.Error!);

        var request = new ServiceRequest
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Address = dto.Address,
            CustomerId = customerId,
            Status = RequestStatus.Pending
        };

        await _requestRepository.AddAsync(request);
        return Result<ServiceRequestDto>.Success(MapToDto(request));
    }

    public async Task<Result<IEnumerable<ServiceRequestDto>>> GetMyRequestsAsync(Guid customerId)
    {
        var requests = await _requestRepository.GetByCustomerIdAsync(customerId);
        return Result<IEnumerable<ServiceRequestDto>>.Success(requests.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<ServiceRequestDto>>> GetAllPendingAsync()
    {
        var requests = await _requestRepository.GetAllPendingAsync();
        return Result<IEnumerable<ServiceRequestDto>>.Success(requests.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<ServiceRequestDto>>> GetNearbyAsync(NearbyRequestsQuery query)
    {
        var results = await _requestRepository.GetNearbyAsync(query.Latitude, query.Longitude, query.RadiusKm);
        
        var dtos = results.Select(r => 
        {
            var dto = MapToDto(r.Request);
            dto.DistanceKm = Math.Round(r.DistanceKm, 2);
            return dto;
        });

        return Result<IEnumerable<ServiceRequestDto>>.Success(dtos);
    }

    public async Task<Result<ServiceRequestDto>> AcceptAsync(Guid requestId, Guid providerId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null)
            return Result<ServiceRequestDto>.Failure("Service request not found.");

        if (request.Status != RequestStatus.Pending)
            return Result<ServiceRequestDto>.Failure("Request is no longer pending.");

        request.Status = RequestStatus.Accepted;
        request.ProviderId = providerId;
        request.AcceptedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;

        await _requestRepository.UpdateAsync(request);
        return Result<ServiceRequestDto>.Success(MapToDto(request));
    }

    public async Task<Result<ServiceRequestDto>> CompleteAsync(Guid requestId, Guid providerId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null)
            return Result<ServiceRequestDto>.Failure("Service request not found.");

        if (request.ProviderId != providerId)
            return Result<ServiceRequestDto>.Failure("You are not the provider assigned to this request.");

        if (request.Status != RequestStatus.Accepted)
            return Result<ServiceRequestDto>.Failure("Only accepted requests can be completed.");

        request.Status = RequestStatus.Completed;
        request.CompletedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;

        await _requestRepository.UpdateAsync(request);
        return Result<ServiceRequestDto>.Success(MapToDto(request));
    }

    public async Task<Result> CancelAsync(Guid requestId, Guid customerId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null)
            return Result.Failure("Service request not found.");

        if (request.CustomerId != customerId)
            return Result.Failure("You can only cancel your own requests.");

        if (request.Status != RequestStatus.Pending)
            return Result.Failure("Only pending requests can be cancelled.");

        request.Status = RequestStatus.Cancelled;
        request.UpdatedAt = DateTime.UtcNow;

        await _requestRepository.UpdateAsync(request);
        return Result.Success();
    }

    private static ServiceRequestDto MapToDto(ServiceRequest entity)
    {
        return new ServiceRequestDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            AiDescription = entity.AiDescription,
            Status = entity.Status.ToString(),
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            Address = entity.Address,
            CustomerId = entity.CustomerId,
            CustomerName = entity.Customer?.Name ?? "Unknown",
            ProviderId = entity.ProviderId,
            ProviderName = entity.Provider?.Name,
            CreatedAt = entity.CreatedAt,
            AcceptedAt = entity.AcceptedAt,
            CompletedAt = entity.CompletedAt
        };
    }
}
