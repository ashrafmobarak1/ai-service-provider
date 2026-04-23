using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMarketplace.API.Attributes;
using ServiceMarketplace.Application.Common.Interfaces;
using ServiceMarketplace.Application.Requests.DTOs;
using ServiceMarketplace.Application.Requests.Interfaces;

namespace ServiceMarketplace.API.Controllers;

[ApiController]
[Route("api/requests")]
[Authorize]
public class ServiceRequestController : ControllerBase
{
    private readonly IServiceRequestService _requestService;
    private readonly ICurrentUser _currentUser;

    public ServiceRequestController(
        IServiceRequestService requestService,
        ICurrentUser currentUser)
    {
        _requestService = requestService;
        _currentUser = currentUser;
    }

    /// <summary>Customer: create a new service request.</summary>
    [HttpPost]
    [RequiresPermission("request.create")]
    [ProducesResponseType(typeof(ServiceRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateRequestDto dto)
    {
        var result = await _requestService.CreateAsync(dto, _currentUser.Id);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>Customer: view own requests.</summary>
    [HttpGet("my")]
    [RequiresPermission("request.view_own")]
    [ProducesResponseType(typeof(IEnumerable<ServiceRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine()
    {
        var result = await _requestService.GetMyRequestsAsync(_currentUser.Id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Provider/Admin: view all pending requests.</summary>
    [HttpGet("all")]
    [RequiresPermission("request.view_all")]
    [ProducesResponseType(typeof(IEnumerable<ServiceRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _requestService.GetAllPendingAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Provider: get pending requests within radius.</summary>
    [HttpGet("nearby")]
    [RequiresPermission("request.view_all")]
    [ProducesResponseType(typeof(IEnumerable<ServiceRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNearby([FromQuery] NearbyRequestsQuery query)
    {
        var result = await _requestService.GetNearbyAsync(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Get a single request by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
        => throw new NotImplementedException();

    /// <summary>Provider: accept a pending request.</summary>
    [HttpPut("{id:guid}/accept")]
    [RequiresPermission("request.accept")]
    [ProducesResponseType(typeof(ServiceRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Accept(Guid id)
    {
        var result = await _requestService.AcceptAsync(id, _currentUser.Id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Provider: complete an accepted request.</summary>
    [HttpPut("{id:guid}/complete")]
    [RequiresPermission("request.complete")]
    [ProducesResponseType(typeof(ServiceRequestDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Complete(Guid id)
    {
        var result = await _requestService.CompleteAsync(id, _currentUser.Id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Customer: cancel a pending request.</summary>
    [HttpDelete("{id:guid}")]
    [RequiresPermission("request.cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _requestService.CancelAsync(id, _currentUser.Id);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
