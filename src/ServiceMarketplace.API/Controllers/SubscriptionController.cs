using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMarketplace.Application.Common.Interfaces;
using ServiceMarketplace.Application.Subscriptions.DTOs;
using ServiceMarketplace.Application.Subscriptions.Interfaces;

namespace ServiceMarketplace.API.Controllers;

[ApiController]
[Route("api/subscriptions")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ICurrentUser _currentUser;

    public SubscriptionController(
        ISubscriptionService subscriptionService,
        ICurrentUser currentUser)
    {
        _subscriptionService = subscriptionService;
        _currentUser = currentUser;
    }

    /// <summary>View the current user's subscription status and usage.</summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus()
    {
        var result = await _subscriptionService.GetStatusAsync(_currentUser.Id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Simulate upgrading the subscription tier (no real payment).</summary>
    [HttpPost("upgrade")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upgrade([FromBody] UpgradeSubscriptionDto dto)
    {
        var result = await _subscriptionService.UpgradeAsync(_currentUser.Id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
