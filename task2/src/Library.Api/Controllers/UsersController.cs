using Library.Application.DTOs;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet("top-borrowers")]
    [ProducesResponseType(typeof(IReadOnlyList<TopBorrowerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopBorrowers(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int take = 10,
        CancellationToken ct = default)
    {
        var end = to ?? DateTime.UtcNow;
        var start = from ?? end.AddDays(-30);

        var result = await _userService.GetTopBorrowersAsync(start, end, take, ct);
        return Ok(result);
    }

    [HttpGet("{userId:int}/history")]
    [ProducesResponseType(typeof(IReadOnlyList<BorrowHistoryGroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBorrowHistory(
        int userId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string groupBy = "Month",
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<TimeGrouping>(groupBy, true, out var grouping))
        {
            return BadRequest("groupBy must be one of: Day, Week, Month.");
        }

        var end = to ?? DateTime.UtcNow;
        var start = from ?? end.AddMonths(-6);

        var history = await _userService.GetBorrowHistoryAsync(userId, start, end, grouping, ct);
        return Ok(history);
    }
}
