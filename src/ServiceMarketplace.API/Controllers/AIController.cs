using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMarketplace.Application.AI.DTOs;
using ServiceMarketplace.Application.AI.Interfaces;

namespace ServiceMarketplace.API.Controllers;

[ApiController]
[Route("api/requests")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;

    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }

    /// <summary>
    /// Enhance a service request's description using Claude AI.
    /// Non-blocking — if AI fails, the original description is unchanged.
    /// </summary>
    [HttpPost("{id:guid}/enhance")]
    [ProducesResponseType(typeof(EnhanceDescriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Enhance(Guid id)
    {
        var result = await _aiService.EnhanceDescriptionAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
