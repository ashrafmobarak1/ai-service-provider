using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.Requests.DTOs;

namespace ServiceMarketplace.Application.Requests.Interfaces;

public interface IServiceRequestService
{
    Task<Result<ServiceRequestDto>> CreateAsync(CreateRequestDto dto, Guid customerId);

    Task<Result<IEnumerable<ServiceRequestDto>>> GetMyRequestsAsync(Guid customerId);

    Task<Result<IEnumerable<ServiceRequestDto>>> GetAllPendingAsync();

    Task<Result<IEnumerable<ServiceRequestDto>>> GetNearbyAsync(NearbyRequestsQuery query);

    Task<Result<ServiceRequestDto>> AcceptAsync(Guid requestId, Guid providerId);

    Task<Result<ServiceRequestDto>> CompleteAsync(Guid requestId, Guid providerId);

    Task<Result> CancelAsync(Guid requestId, Guid customerId);
}
